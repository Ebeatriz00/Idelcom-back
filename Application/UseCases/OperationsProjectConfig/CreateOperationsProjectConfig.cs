using Application.DTOs.OperationsProjectConfing;
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
    public class CreateOperationsProjectConfig(
        IOperationsProjectConfigRepository repository,
        IAuditService auditService,
        IAuditLogFactory auditLogFactory,
        IMapper mapper,
        ISqlConnectionFactory sqlConnectionFactory,
        IValidator<OperationsProjectConfigCreateDto> validator)
    {
        private readonly IOperationsProjectConfigRepository _repository = repository;
        private readonly IAuditService _auditService = auditService;
        private readonly IAuditLogFactory _auditLogFactory = auditLogFactory;
        private readonly IMapper _mapper = mapper;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;
        private readonly IValidator<OperationsProjectConfigCreateDto> _validator = validator;

        public async Task<GlobalResponse> ExecuteAsync(OperationsProjectConfigCreateDto dto, long userId, long businessId)
        {
            dto.BusinessId = businessId;
            dto.CreateUser = userId;
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
                var entity = _mapper.Map<Core.Entities.Operations.OperationsProjectConfig>(dto);
                entity.BusinessId = businessId;
                entity.CreateUser = userId;
                var result = await _repository.AddAsync(entity, transaction);

                if (result.Id == null)
                {
                    throw new InvalidOperationException("No se pudo crear la configuración del proyecto de operaciones.");
                }

                entity.OperationsProjectConfigId = (long)result.Id;
                entity.BusinessId = (long)businessId;

                var auditLog = _auditLogFactory.Create(
                   businessId,
                   TableNames.OperationsProjectConfig,
                   (long)result.Id,
                   userId);

                await _auditService.RegisterCreateAsync(entity, auditLog, transaction);

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