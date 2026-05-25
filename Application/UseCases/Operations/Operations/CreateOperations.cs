using Application.DTOs.Operations.Operations;
using AutoMapper;
using Core.Entities.Operations;
using Core.Interfaces.Audit;
using Core.Interfaces.Operations;
using FluentValidation;
using Infrastructure.Persistence;
using SharedKernel;
using SharedKernel.Constants;
using System.Data;
using AppValidationException = Application.Exceptions.ValidationException;

namespace Application.UseCases.Operations.Operations
{
    public class CreateOperations(
        IOperationsRepository repository,
        IAuditService auditService,
        IAuditLogFactory auditLogFactory,
        IMapper mapper,
        ISqlConnectionFactory sqlConnectionFactory,
        IValidator<OperationsCreateDto> validator
        )
    {
        private readonly IOperationsRepository _repository = repository;
        private readonly IAuditService _auditService = auditService;
        private readonly IAuditLogFactory _auditLogFactory = auditLogFactory;
        private readonly IMapper _mapper = mapper;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;
        private readonly IValidator<OperationsCreateDto> _validator = validator;

        public async Task<BaseResponseId> ExecuteAsync(OperationsCreateDto dto, long userId, long businessId)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errors = validation.Errors
                    .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                    .ToList();

                throw new AppValidationException(errors);
            }

            // Begin transaction
            using var connection = _sqlConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                // Map DTO to entity
                var entity = _mapper.Map<Operation>(dto);

                // Create the operation
                var created = await _repository.CreateAsync(entity, userId, businessId, transaction);

                // Check if creation was successful
                if (created.Id == null)
                {
                    throw new Exception("Failed to create operation.");
                }

                entity.OperationsId = (long)created.Id;
                entity.BusinessId = (long)businessId;

                // Register audit log
                var auditLog = _auditLogFactory.Create(
                    businessId,
                    TableNames.Operations,
                    (long)created.Id,
                    userId);

                await _auditService.RegisterCreateAsync(entity, auditLog, trans: transaction);

                transaction.Commit();
                return created;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new InvalidOperationException(ex.Message);
            }
        }
    }
}
