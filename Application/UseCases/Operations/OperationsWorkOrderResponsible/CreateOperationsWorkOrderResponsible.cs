using Application.DTOs.Operations.OperationsWorkOrderResponsible;
using AutoMapper;
using Core.Entities.Operations.Core.Entities;
using Core.Interfaces.Audit;
using Core.Interfaces.Operations;
using FluentValidation;
using Infrastructure.Persistence;
using SharedKernel;
using SharedKernel.Constants;
using System.Data;
using AppValidationException = Application.Exceptions.ValidationException;

namespace Application.UseCases.Operations.OperationsWorkOrderResponsible
{
    public class CreateOperationsWorkOrderResponsible(
        IOperationsWorkOrderResponsibleRepository repository,
        IAuditService auditService,
        IAuditLogFactory auditLogFactory,
        IMapper mapper,
        ISqlConnectionFactory sqlConnectionFactory,
        IValidator<OperationsWorkOrderResponsibleCreateDto> validator)
    {
        private readonly IOperationsWorkOrderResponsibleRepository _repository = repository;
        private readonly IAuditService _auditService = auditService;
        private readonly IAuditLogFactory _auditLogFactory = auditLogFactory;
        private readonly IMapper _mapper = mapper;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;
        private readonly IValidator<OperationsWorkOrderResponsibleCreateDto> _validator = validator;

        public async Task<BaseResponseId> ExecuteAsync(
            OperationsWorkOrderResponsibleCreateDto dto,
            long userId,
            long businessId)
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
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                var entity = _mapper.Map<OperationWorkOrderResponsible>(dto);
                var result = await _repository.CreateAsync(entity, userId, businessId, transaction);

                if (result.Id == null)
                    throw new InvalidOperationException("No se pudo crear el registro.");

                entity.WorkOrderResponsibleId = (long)result.Id;
                entity.BusinessId = businessId;

                var auditLog = _auditLogFactory.Create(
                    businessId,
                    TableNames.OperationsWorkOrderResponsible,
                    (long)result.Id,
                    userId);

                await _auditService.RegisterCreateAsync(entity, auditLog, transaction);

                await transaction.CommitAsync();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }

}
