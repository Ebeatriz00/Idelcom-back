using Application.DTOs.SsomaHomologationPersonnel;
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

namespace Application.UseCases.SsomaHomologationPersonnel
{
    public class UpdateSsomaHomologationPersonnel(
        ISsomaHomologationPersonnelRepository repository,
        IAuditService auditService,
        IAuditLogFactory auditLogFactory,
        IMapper mapper,
        ISqlConnectionFactory sqlConnectionFactory,
        IValidator<SsomaHomologationPersonnelUpdateDto> validator,
        SsomaHomologationPersonnelBusinessRules businessRules)
    {
        private readonly ISsomaHomologationPersonnelRepository _repository = repository;
        private readonly IAuditService _auditService = auditService;
        private readonly IAuditLogFactory _auditLogFactory = auditLogFactory;
        private readonly IMapper _mapper = mapper;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;
        private readonly IValidator<SsomaHomologationPersonnelUpdateDto> _validator = validator;
        private readonly SsomaHomologationPersonnelBusinessRules _businessRules = businessRules;

        public async Task<BaseResponse> ExecuteAsync(SsomaHomologationPersonnelUpdateDto dto, long userId, long businessId)
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
                var before = await _repository.GetByIdAsync(dto.HomologationPersonnelId, businessId);
                if (before == null)
                    throw new BusinessException("No se encontró la homologación de personal SSOMA.");

                _businessRules.Normalize(
                    dto.OperationsId,
                    dto.ValidFrom!.Value,
                    dto.ValidTo!.Value,
                    dto.Notes,
                    out var normalizedOperationsId,
                    out var normalizedValidFrom,
                    out var normalizedValidTo,
                    out var normalizedNotes);

                dto.OperationsId = normalizedOperationsId;
                dto.ValidFrom = normalizedValidFrom;
                dto.ValidTo = normalizedValidTo;
                dto.Notes = normalizedNotes;

                await _businessRules.ValidateReferencesAsync(
                    businessId,
                    dto.WorkerId,
                    dto.WorkerStatusId,
                    dto.MedicalAptitudeId,

                    dto.OperationsId);

                var entity = _mapper.Map<Core.Entities.Ssoma.SsomaHomologationPersonnel>(dto);
                entity.BusinessId = businessId;
                entity.UpdateUser = userId;

                var updated = await _repository.UpdateAsync(entity, transaction);

                var after = await _repository.GetByIdAsync(dto.HomologationPersonnelId, businessId, transaction);
                if (after == null)
                    throw new BusinessException("No se pudo recuperar la homologación de personal SSOMA actualizada.");

                var auditLog = _auditLogFactory.Create(
                    businessId,
                    TableNames.SsomaHomologationPersonnel,
                    before.HomologationPersonnelId,
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
                throw new DatabaseException("Error al actualizar la homologación de personal SSOMA.", ex.Message);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new DatabaseException("Error inesperado al actualizar la homologación de personal SSOMA.", ex.Message);
            }
        }
    }
}
