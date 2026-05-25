using Application.DTOs.SsomaOperationsRequirement;
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

namespace Application.UseCases.SsomaOperationsRequirement
{
    public class UpdateSsomaOperationsRequirement(
        ISsomaOperationsRequirementRepository repository,
        IAuditService auditService,
        IAuditLogFactory auditLogFactory,
        IMapper mapper,
        ISqlConnectionFactory sqlConnectionFactory,
        IValidator<SsomaOperationsRequirementUpdateDto> validator)
    {
        private readonly ISsomaOperationsRequirementRepository _repository = repository;
        private readonly IAuditService _auditService = auditService;
        private readonly IAuditLogFactory _auditLogFactory = auditLogFactory;
        private readonly IMapper _mapper = mapper;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;
        private readonly IValidator<SsomaOperationsRequirementUpdateDto> _validator = validator;

        public async Task<BaseResponse> ExecuteAsync(SsomaOperationsRequirementUpdateDto dto, long userId, long businessId)
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
                var before = await _repository.GetByIdAsync(dto.SsomaOperationsRequirementId, businessId);
                if (before == null)
                    throw new Exception("No se encontró el requerimiento SSOMA por operación.");

                var entity = _mapper.Map<Core.Entities.Ssoma.SsomaOperationsRequirement>(dto);
                entity.BusinessId = businessId;
                entity.UpdateUser = userId;

                var updated = await _repository.UpdateAsync(entity, transaction);

                var after = await _repository.GetByIdAsync(dto.SsomaOperationsRequirementId, businessId, transaction);
                if (after == null)
                    throw new Exception("No se pudo recuperar el requerimiento SSOMA por operación actualizado.");

                var auditLog = _auditLogFactory.Create(
                    businessId,
                    TableNames.SsomaOperationsRequirement,
                    before.SsomaOperationsRequirementId,
                    userId);

                await _auditService.RegisterUpdateAsync(before, after, auditLog, transaction);

                transaction.Commit();
                return updated;
            }
            catch (SqlException ex)
            {
                transaction.Rollback();
                throw new DatabaseException("Error al actualizar requerimientos SSOMA por operación.", ex.Message);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new DatabaseException("Error inesperado al actualizar requerimientos SSOMA por operación.", ex.Message);
            }
        }
    }
}
