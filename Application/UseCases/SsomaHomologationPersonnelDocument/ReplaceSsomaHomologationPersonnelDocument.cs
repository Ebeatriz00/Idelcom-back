using Application.DTOs.SsomaHomologationPersonnelDocument;
using Core.Interfaces.Audit;
using Core.Interfaces.Ssoma;
using FluentValidation;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using SharedKernel;
using SharedKernel.Constants;
using System.Data;
using AppValidationException = Application.Exceptions.ValidationException;

namespace Application.UseCases.SsomaHomologationPersonnelDocument
{
    public class ReplaceSsomaHomologationPersonnelDocument(
        ISsomaHomologationPersonnelDocumentRepository repository,
        IAuditService auditService,
        IAuditLogFactory auditLogFactory,
        ISqlConnectionFactory sqlConnectionFactory,
        IValidator<SsomaHomologationPersonnelDocumentCreateDto> validator,
        SsomaHomologationPersonnelDocumentBusinessRules businessRules)
    {
        private readonly ISsomaHomologationPersonnelDocumentRepository _repository = repository;
        private readonly IAuditService _auditService = auditService;
        private readonly IAuditLogFactory _auditLogFactory = auditLogFactory;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;
        private readonly IValidator<SsomaHomologationPersonnelDocumentCreateDto> _validator = validator;
        private readonly SsomaHomologationPersonnelDocumentBusinessRules _businessRules = businessRules;
        private const int ValidationStatusValid = 2;

        public async Task<BaseResponseId> ExecuteAsync(SsomaHomologationPersonnelDocumentReplaceDto dto, long userId, long businessId)
        {
            using var connection = _sqlConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var created = await ReplaceOneAsync(dto, userId, businessId, transaction);
                transaction.Commit();
                return created;
            }
            catch (AppValidationException)
            {
                transaction.Rollback();
                throw;
            }
            catch (BaseException)
            {
                transaction.Rollback();
                throw;
            }
            catch (SqlException ex)
            {
                transaction.Rollback();
                throw new DatabaseException("Error al reemplazar el documento de homologacion de personal SSOMA.", ex.Message);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new DatabaseException("Error inesperado al reemplazar el documento de homologacion de personal SSOMA.", ex.Message);
            }
        }

        public async Task<SsomaHomologationPersonnelDocumentReplaceResponseDto> ExecuteAsync(
            SsomaHomologationPersonnelDocumentReplaceRequestDto dto,
            long userId,
            long businessId)
        {
            var documents = dto.Documents is { Count: > 0 }
                ? dto.Documents
                : new List<SsomaHomologationPersonnelDocumentReplaceDto> { dto };

            using var connection = _sqlConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var results = new List<BaseResponseId>();
                foreach (var document in documents)
                {
                    results.Add(await ReplaceOneAsync(document, userId, businessId, transaction));
                }

                transaction.Commit();
                return new SsomaHomologationPersonnelDocumentReplaceResponseDto
                {
                    Status = 1,
                    Message = results.Count == 1
                        ? "Documento reemplazado correctamente."
                        : "Documentos reemplazados correctamente.",
                    Documents = results
                };
            }
            catch (AppValidationException)
            {
                transaction.Rollback();
                throw;
            }
            catch (BaseException)
            {
                transaction.Rollback();
                throw;
            }
            catch (SqlException ex)
            {
                transaction.Rollback();
                throw new DatabaseException("Error al reemplazar los documentos de homologacion de personal SSOMA.", ex.Message);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new DatabaseException("Error inesperado al reemplazar los documentos de homologacion de personal SSOMA.", ex.Message);
            }
        }

        private async Task<BaseResponseId> ReplaceOneAsync(
            SsomaHomologationPersonnelDocumentReplaceDto dto,
            long userId,
            long businessId,
            IDbTransaction transaction)
        {
            var documentToReplace = dto.SsomaHomologationPersonnelDocumentId is > 0
                ? await _repository.GetByIdAsync(dto.SsomaHomologationPersonnelDocumentId.Value, businessId, transaction)
                : await _repository.GetActiveByHomologationAndRequirementAsync(
                    businessId,
                    dto.HomologationPersonnelId,
                    dto.RequirementId,
                    transaction);

            if (documentToReplace == null)
                throw new BusinessException("No se encontro el documento de homologacion de personal SSOMA a reemplazar.");

            _businessRules.Normalize(
                dto.FileName,
                dto.FileUrl,
                dto.FilePath,
                dto.IssueDate,
                dto.ExpirationDate,
                dto.ReviewDate,
                dto.Observation,
                out var normalizedFileName,
                out var normalizedFileUrl,
                out var normalizedFilePath,
                out var normalizedIssueDate,
                out var normalizedExpirationDate,
                out var normalizedReviewDate,
                out var normalizedObservation);

            var replacementReason = dto.ReplacementReason?.Trim();
            if (replacementReason?.Length > 1000)
                throw new BusinessException("El motivo del reemplazo no puede exceder 1000 caracteres.");

            var createDto = new SsomaHomologationPersonnelDocumentCreateDto
            {
                HomologationPersonnelId = documentToReplace.HomologationPersonnelId,
                RequirementId = documentToReplace.RequirementId,
                FileName = normalizedFileName,
                FileUrl = normalizedFileUrl,
                FilePath = normalizedFilePath,
                IssueDate = normalizedIssueDate,
                ExpirationDate = normalizedExpirationDate,
                ValidationStatusId = ValidationStatusValid,
                ReviewDate = normalizedReviewDate,
                Observation = normalizedObservation,
                ReplacedDocumentId = documentToReplace.SsomaHomologationPersonnelDocumentId,
                DocumentVersion = (documentToReplace.DocumentVersion ?? 1) + 1,
                ReplacementReason = replacementReason
            };

            var validation = await _validator.ValidateAsync(createDto);
            if (!validation.IsValid)
            {
                var errors = validation.Errors
                    .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                    .ToList();

                throw new AppValidationException(errors);
            }

            await _businessRules.ValidateReferencesAsync(
                businessId,
                createDto.HomologationPersonnelId,
                createDto.RequirementId,
                createDto.FileName,
                createDto.IssueDate,
                createDto.ExpirationDate,
                createDto.ReviewDate);

            createDto.ExpirationDate = await _businessRules.ResolveExpirationDateAsync(
                businessId,
                createDto.RequirementId,
                createDto.IssueDate,
                createDto.ExpirationDate);

            var newDocument = new Core.Entities.Ssoma.SsomaHomologationPersonnelDocument
            {
                BusinessId = businessId,
                HomologationPersonnelId = createDto.HomologationPersonnelId,
                RequirementId = createDto.RequirementId,
                FileName = createDto.FileName,
                FileUrl = createDto.FileUrl,
                FilePath = createDto.FilePath,
                IssueDate = createDto.IssueDate,
                ExpirationDate = createDto.ExpirationDate,
                ValidationStatusId = createDto.ValidationStatusId,
                ReviewDate = createDto.ReviewDate,
                Observation = createDto.Observation,
                ReplacedDocumentId = createDto.ReplacedDocumentId,
                DocumentVersion = createDto.DocumentVersion,
                ReplacementReason = createDto.ReplacementReason,
                CreateUser = userId
            };

            var created = await _repository.CreateAsync(newDocument, transaction);
            if (created.Id == null || created.Id <= 0)
                throw new Exception("No se pudo crear el documento reemplazante de homologacion de personal SSOMA.");

            newDocument.SsomaHomologationPersonnelDocumentId = created.Id.Value;

            await _repository.MarkAsReplacedAsync(
                documentToReplace.SsomaHomologationPersonnelDocumentId,
                businessId,
                created.Id.Value,
                replacementReason,
                userId,
                transaction);

            var createAuditLog = _auditLogFactory.Create(
                businessId,
                TableNames.SsomaHomologationPersonnelDocument,
                created.Id.Value,
                userId);

            await _auditService.RegisterCreateAsync(newDocument, createAuditLog, transaction);

            var replacedDocumentAfter = CopyDocument(documentToReplace);
            replacedDocumentAfter.ReplacedDocumentId = created.Id.Value;
            replacedDocumentAfter.ReplacementReason = replacementReason;
            replacedDocumentAfter.UpdateUser = userId;
            replacedDocumentAfter.UpdateDate = DateTime.Now;
            replacedDocumentAfter.Status = "0";

            var updateAuditLog = _auditLogFactory.Create(
                businessId,
                TableNames.SsomaHomologationPersonnelDocument,
                documentToReplace.SsomaHomologationPersonnelDocumentId,
                userId);

            await _auditService.RegisterUpdateAsync(documentToReplace, replacedDocumentAfter, updateAuditLog, transaction);
            return created;
        }

        private static Core.Entities.Ssoma.SsomaHomologationPersonnelDocument CopyDocument(
            Core.Entities.Ssoma.SsomaHomologationPersonnelDocument source)
        {
            return new Core.Entities.Ssoma.SsomaHomologationPersonnelDocument
            {
                SsomaHomologationPersonnelDocumentId = source.SsomaHomologationPersonnelDocumentId,
                BusinessId = source.BusinessId,
                HomologationPersonnelId = source.HomologationPersonnelId,
                RequirementId = source.RequirementId,
                FileName = source.FileName,
                FileUrl = source.FileUrl,
                FilePath = source.FilePath,
                IssueDate = source.IssueDate,
                ExpirationDate = source.ExpirationDate,
                ValidationStatusId = source.ValidationStatusId,
                ReviewDate = source.ReviewDate,
                Observation = source.Observation,
                ReplacedDocumentId = source.ReplacedDocumentId,
                DocumentVersion = source.DocumentVersion,
                ReplacementReason = source.ReplacementReason,
                CreateDate = source.CreateDate,
                CreateUser = source.CreateUser,
                UpdateDate = source.UpdateDate,
                UpdateUser = source.UpdateUser,
                Status = source.Status
            };
        }
    }
}
