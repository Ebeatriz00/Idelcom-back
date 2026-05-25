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
        SsomaHomologationPersonnelDocumentBusinessRules businessRules)
    {
        private readonly ISsomaHomologationPersonnelDocumentRepository _repository = repository;
        private readonly IAuditService _auditService = auditService;
        private readonly IAuditLogFactory _auditLogFactory = auditLogFactory;
        private readonly IMapper _mapper = mapper;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;
        private readonly IValidator<SsomaHomologationPersonnelDocumentCreateDto> _validator = validator;
        private readonly SsomaHomologationPersonnelDocumentBusinessRules _businessRules = businessRules;

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

                var existing = await _repository.GetActiveByHomologationAndRequirementAsync(
                    businessId,
                    entity.HomologationPersonnelId,
                    entity.RequirementId,
                    transaction);

                if (existing != null)
                {
                    entity.SsomaHomologationPersonnelDocumentId = existing.SsomaHomologationPersonnelDocumentId;
                    entity.UpdateUser = userId;

                    var updated = await _repository.UpdateAsync(entity, transaction);

                    var after = await _repository.GetByIdAsync(existing.SsomaHomologationPersonnelDocumentId, businessId, transaction);
                    if (after == null)
                        throw new BusinessException("No se pudo recuperar el documento de homologacion de personal SSOMA actualizado.");

                    var updateAuditLog = _auditLogFactory.Create(
                        businessId,
                        TableNames.SsomaHomologationPersonnelDocument,
                        existing.SsomaHomologationPersonnelDocumentId,
                        userId);

                    await _auditService.RegisterUpdateAsync(existing, after, updateAuditLog, transaction);

                    transaction.Commit();
                    return new BaseResponseId
                    {
                        Status = updated.Status,
                        Message = updated.Message,
                        Id = existing.SsomaHomologationPersonnelDocumentId
                    };
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
