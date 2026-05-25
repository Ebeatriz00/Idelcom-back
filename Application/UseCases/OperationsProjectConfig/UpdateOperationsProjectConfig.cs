using Application.DTOs.OperationsProjectConfing;
using Application.Exceptions;
using AutoMapper;
using Core.Entities.Audit;
using Core.Interfaces.Audit;
using Core.Interfaces.Operations;
using FluentValidation;
using Infrastructure.Persistence;
using SharedKernel;
using SharedKernel.Constants;
using AppValidationException = Application.Exceptions.ValidationException;

namespace Application.UseCases.OperationsProjectConfig
{
    public class UpdateOperationsProjectConfig(
        IOperationsProjectConfigRepository repository,
        IAuditService auditService,
        IAuditLogFactory auditLogFactory,
        IMapper mapper,
        ISqlConnectionFactory sqlConnectionFactory,
        IValidator<OperationsProjectConfigUpdateDto> validator)
    {
        private readonly IOperationsProjectConfigRepository _repository = repository;
        private readonly IAuditService _auditService = auditService;
        private readonly IAuditLogFactory _auditLogFactory = auditLogFactory;
        private readonly IMapper _mapper = mapper;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;
        private readonly IValidator<OperationsProjectConfigUpdateDto> _validator = validator;

        public async Task<GlobalResponse> ExecuteAsync(OperationsProjectConfigUpdateDto dto, long userId, long businessId)
        {
            dto.BusinessId = businessId;
            dto.UpdateUser = userId;
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
                var before = await _repository.GetByIdAsync(dto.OperationsProjectConfigId,dto.OperationsId);

                if (before is null)
                {
                    throw new InvalidOperationException("No se encontró la configuración del proyecto de operaciones.");
                }

                var entity = _mapper.Map<Core.Entities.Operations.OperationsProjectConfig>(dto);
                entity.BusinessId = businessId;
                entity.UpdateUser = userId;


                var result = await _repository.UpdateAsync(entity, transaction);
                var after = await _repository.GetByIdAsync(dto.OperationsProjectConfigId, dto.OperationsId, transaction);

                var audilog = _auditLogFactory.Create(
                   businessId,
                   TableNames.OperationsWorkOrder,
                   before.OperationsProjectConfigId,
                   userId);

                await _auditService.RegisterUpdateAsync(before, after, audilog, transaction);
                transaction.Commit();
                return result;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}