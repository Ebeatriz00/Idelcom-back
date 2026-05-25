using Application.DTOs.SsomaRequirement;
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

namespace Application.UseCases.SsomaRequirement
{
    public class UpdateSsomaRequirement(
        ISsomaRequirementRepository repository,
        IAuditService auditService,
        IAuditLogFactory auditLogFactory,
        IMapper mapper,
        ISqlConnectionFactory sqlConnectionFactory,
        IValidator<SsomaRequirementUpdateDto> validator)
    {
        private readonly ISsomaRequirementRepository _repository = repository;
        private readonly IAuditService _auditService = auditService;
        private readonly IAuditLogFactory _auditLogFactory = auditLogFactory;
        private readonly IMapper _mapper = mapper;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;
        private readonly IValidator<SsomaRequirementUpdateDto> _validator = validator;

        public async Task<BaseResponse> ExecuteAsync(SsomaRequirementUpdateDto dto, long userId, long businessId)
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
                var before = await _repository.GetByIdAsync(dto.RequirementId, businessId);
                if (before == null)
                    throw new Exception("No se encontró el requerimiento SSOMA.");

                var entity = _mapper.Map<Core.Entities.Ssoma.SsomaRequirement>(dto);
                entity.BusinessId = businessId;
                entity.UpdateUser = userId;

                var updated = await _repository.UpdateAsync(entity, transaction);

                var after = await _repository.GetByIdAsync(dto.RequirementId, businessId, transaction);
                if (after == null)
                    throw new Exception("No se pudo recuperar el requerimiento SSOMA actualizado.");

                var auditLog = _auditLogFactory.Create(
                    businessId,
                    TableNames.SsomaRequirement,
                    before.RequirementId,
                    userId);

                await _auditService.RegisterUpdateAsync(before, after, auditLog, transaction);

                transaction.Commit();
                return updated;
            }
            catch (SqlException ex)
            {
                transaction.Rollback();
                throw new DatabaseException("Error al actualizar requerimientos SSOMA.", ex.Message);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new DatabaseException("Error inesperado al actualizar requerimientos SSOMA.", ex.Message);
            }
        }
    }
}
