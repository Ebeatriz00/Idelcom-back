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
    public class CreateSsomaOperationsRequirement(
        ISsomaOperationsRequirementRepository repository,
        IAuditService auditService,
        IAuditLogFactory auditLogFactory,
        IMapper mapper,
        ISqlConnectionFactory sqlConnectionFactory,
        IValidator<SsomaOperationsRequirementCreateDto> validator)
    {
        private readonly ISsomaOperationsRequirementRepository _repository = repository;
        private readonly IAuditService _auditService = auditService;
        private readonly IAuditLogFactory _auditLogFactory = auditLogFactory;
        private readonly IMapper _mapper = mapper;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;
        private readonly IValidator<SsomaOperationsRequirementCreateDto> _validator = validator;

        public async Task<BaseResponseId> ExecuteAsync(SsomaOperationsRequirementCreateDto dto, long userId, long businessId)
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
                var entity = _mapper.Map<Core.Entities.Ssoma.SsomaOperationsRequirement>(dto);
                entity.BusinessId = businessId;
                entity.CreateUser = userId;

                var created = await _repository.CreateAsync(entity, transaction);

                if (created.Id == null || created.Id <= 0)
                    throw new Exception("No se pudo crear el requerimiento SSOMA por operación.");

                var auditLog = _auditLogFactory.Create(
                    businessId,
                    TableNames.SsomaOperationsRequirement,
                    (long)created.Id,
                    userId);

                entity.SsomaOperationsRequirementId = (long)created.Id;
                await _auditService.RegisterCreateAsync(entity, auditLog, trans: transaction);

                transaction.Commit();
                return created;
            }
            catch (SqlException ex)
            {
                transaction.Rollback();
                throw new DatabaseException("Error al registrar requerimientos SSOMA por operación.", ex.Message);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new DatabaseException("Error inesperado al guardar requerimientos SSOMA por operación.", ex.Message);
            }
        }
    }
}
