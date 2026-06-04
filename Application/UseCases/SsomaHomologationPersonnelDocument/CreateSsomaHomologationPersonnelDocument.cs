using Application.DTOs.SsomaHomologationPersonnelDocument;
using AutoMapper;
using Core.Interfaces.Audit;
using Core.Interfaces.Ssoma;
using FluentValidation;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using SharedKernel;
using SharedKernel.Constants;
using AppValidationException = Application.Exceptions.ValidationException;

namespace Application.UseCases.SsomaHomologationPersonnelDocument
{
    public class CreateSsomaHomologationPersonnelDocument(
        ISsomaHomologationPersonnelDocumentRepository repository,
        IAuditService auditService,
        IAuditLogFactory auditLogFactory,
        IMapper mapper,
        ISqlConnectionFactory sqlConnectionFactory,
        IValidator<SsomaHomologationPersonnelDocumentCreateDto> validator,
        SsomaHomologationPersonnelDocumentBusinessRules businessRules,
        Core.Interfaces.IStorageService storageService)
    {
        private readonly ISsomaHomologationPersonnelDocumentRepository _repository = repository;
        private readonly IAuditService _auditService = auditService;
        private readonly IAuditLogFactory _auditLogFactory = auditLogFactory;
        private readonly IMapper _mapper = mapper;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;
        private readonly IValidator<SsomaHomologationPersonnelDocumentCreateDto> _validator = validator;
        private readonly SsomaHomologationPersonnelDocumentBusinessRules _businessRules = businessRules;
        private readonly Core.Interfaces.IStorageService _storageService = storageService;

        public async Task<BaseResponseId> ExecuteAsync(SsomaHomologationPersonnelDocumentCreateDto dto, long userId, long businessId)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errors = validation.Errors
                    .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                    .ToList();

                throw new AppValidationException(errors);
            }

            using var connection = _sqlConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                Guid? fileUid = null;
                if (dto.File != null && dto.File.Length > 0)
                {
                    using var stream = dto.File.OpenReadStream();
                    fileUid = await _storageService.UploadAsync(
                        stream,
                        dto.File.FileName,
                        $"SSOMA/HomologacionPersonal/{dto.HomologationPersonnelId}/Requisito/{dto.RequirementId}",
                        userId);
                    dto.FileName = dto.File.FileName;
                    dto.FileUrl = null;
                    dto.FilePath = null;
                }

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

                dto.FileName = normalizedFileName;
                dto.FileUrl = normalizedFileUrl;
                dto.FilePath = normalizedFilePath;
                dto.IssueDate = normalizedIssueDate;
                dto.ExpirationDate = normalizedExpirationDate;
                dto.ReviewDate = normalizedReviewDate;
                dto.Observation = normalizedObservation;

                await _businessRules.ValidateReferencesAsync(
                    businessId,
                    dto.HomologationPersonnelId,
                    dto.RequirementId,
                    dto.FileName,
                    dto.IssueDate,
                    dto.ExpirationDate,
                    dto.ReviewDate);

                dto.ExpirationDate = await _businessRules.ResolveExpirationDateAsync(
                    businessId,
                    dto.RequirementId,
                    dto.IssueDate,
                    dto.ExpirationDate);

                var entity = _mapper.Map<Core.Entities.Ssoma.SsomaHomologationPersonnelDocument>(dto);
                entity.BusinessId = businessId;
                entity.FileUid = fileUid;

                var existing = await _repository.GetActiveByHomologationAndRequirementAsync(
                    businessId,
                    entity.HomologationPersonnelId,
                    entity.RequirementId,
                    transaction);

                if (existing != null)
                {
                    throw new Application.Exceptions.DuplicateEntryException($"Ya existe un documento activo para este requerimiento. Si desea actualizarlo, utilice la opción de reemplazo de documentos.");
                }

                entity.CreateUser = userId;

                var created = await _repository.CreateAsync(entity, transaction);
                if (created.Id == null || created.Id <= 0)
                    throw new Exception("No se pudo crear el documento de homologación de personal SSOMA.");

                var auditLog = _auditLogFactory.Create(
                    businessId,
                    TableNames.SsomaHomologationPersonnelDocument,
                    (long)created.Id,
                    userId);

                entity.SsomaHomologationPersonnelDocumentId = (long)created.Id;
                await _auditService.RegisterCreateAsync(entity, auditLog, trans: transaction);

                transaction.Commit();
                return created;
            }
            catch (BaseException)
            {
                transaction.Rollback();
                throw;
            }
            catch (Application.Exceptions.BaseException)
            {
                transaction.Rollback();
                throw;
            }
            catch (SqlException ex)
            {
                transaction.Rollback();
                throw new DatabaseException("Error al registrar el documento de homologación de personal SSOMA.", ex.Message);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new DatabaseException("Error inesperado al guardar el documento de homologación de personal SSOMA.", ex.Message);
            }
        }
    }
}
