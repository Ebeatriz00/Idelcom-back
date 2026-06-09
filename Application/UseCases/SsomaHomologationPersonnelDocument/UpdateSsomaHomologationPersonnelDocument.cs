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
    public class UpdateSsomaHomologationPersonnelDocument(
        ISsomaHomologationPersonnelDocumentRepository repository,
        IAuditService auditService,
        IAuditLogFactory auditLogFactory,
        IMapper mapper,
        ISqlConnectionFactory sqlConnectionFactory,
        IValidator<SsomaHomologationPersonnelDocumentUpdateDto> validator,
        SsomaHomologationPersonnelDocumentBusinessRules businessRules,
        Core.Interfaces.IStorageService storageService)
    {
        private readonly ISsomaHomologationPersonnelDocumentRepository _repository = repository;
        private readonly IAuditService _auditService = auditService;
        private readonly IAuditLogFactory _auditLogFactory = auditLogFactory;
        private readonly IMapper _mapper = mapper;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;
        private readonly IValidator<SsomaHomologationPersonnelDocumentUpdateDto> _validator = validator;
        private readonly SsomaHomologationPersonnelDocumentBusinessRules _businessRules = businessRules;
        private readonly Core.Interfaces.IStorageService _storageService = storageService;

        public async Task<BaseResponseId> ExecuteAsync(SsomaHomologationPersonnelDocumentUpdateDto dto, long userId, long businessId)
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
                var before = await _repository.GetByIdAsync(dto.SsomaHomologationPersonnelDocumentId, businessId);
                if (before == null)
                    throw new BusinessException("No se encontró el documento de homologación de personal SSOMA.");

                Guid? fileUid = null;
                if (dto.File != null && dto.File.Length > 0)
                {
                    var uploadPath = await _businessRules.GetUploadPathAsync(businessId, dto.HomologationPersonnelId, dto.RequirementId, dto.ClinicId);
                    using var stream = dto.File.OpenReadStream();
                    fileUid = await _storageService.UploadAsync(
                        stream,
                        dto.File.FileName,
                        uploadPath,
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
                entity.UpdateUser = userId;
                entity.FileUid = fileUid;

                var updated = await _repository.UpdateAsync(entity, transaction);
                if (updated.Id == null || updated.Id <= 0)
                    throw new Exception("No se generó el ID versionado del documento.");

                var after = await _repository.GetByIdAsync(updated.Id.Value, businessId, transaction);
                if (after == null)
                    throw new BusinessException("No se pudo recuperar el documento de homologación de personal SSOMA actualizado.");

                var auditLog = _auditLogFactory.Create(
                    businessId,
                    TableNames.SsomaHomologationPersonnelDocument,
                    before.SsomaHomologationPersonnelDocumentId,
                    userId);

                await _auditService.RegisterUpdateAsync(before, after, auditLog, transaction);

                transaction.Commit();
                return updated;
            }
            catch (BaseException)
            {
                transaction.Rollback();
                throw;
            }
            catch (SqlException ex)
            {
                transaction.Rollback();
                throw new DatabaseException("Error al actualizar el documento de homologación de personal SSOMA.", ex.Message);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new DatabaseException("Error inesperado al actualizar el documento de homologación de personal SSOMA.", ex.Message);
            }
        }
    }
}
