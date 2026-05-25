using Application.DTOs.InventoryCount;
using AutoMapper;
using Core.Commands.Logistic;
using Core.Entities.Logistic;
using Core.Interfaces.Audit;
using Core.Interfaces.Logistic;
using FluentValidation;
using Infrastructure.Persistence;
using SharedKernel;
using SharedKernel.Constants;

namespace Application.UseCases.InventoryCount
{
    public class CreateInventoryCountUseCase(
        IInventoryCountRepository repository,
        IValidator<InventoryCountCreateDto> validator,
        ISqlConnectionFactory sqlConnectionFactory,
        IAuditService auditService,
        IAuditLogFactory auditLogFactory,
        IMapper mapper)
    {
        private readonly IInventoryCountRepository _repository = repository;
        private readonly IValidator<InventoryCountCreateDto> _validator = validator;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;
        private readonly IAuditService _auditService = auditService;
        private readonly IAuditLogFactory _auditLogFactory = auditLogFactory;
        private readonly IMapper _mapper = mapper;

        public async Task<BaseResponseId> ExecuteAsync(InventoryCountCreateDto dto, long userId, long businessId)
        {
            await InventoryCountValidation.ValidateAsync(_validator, dto);

            using var connection = _sqlConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var command = _mapper.Map<InventoryCountCreateCommand>(dto);
                command.BusinessId = businessId;
                command.UserId = userId;

                var result = await _repository.CreateAsync(command, transaction);

                if (result.Id is > 0)
                {
                    var created = await _repository.GetByIdAsync(businessId, result.Id.Value, transaction);
                    if (created?.Header is not null)
                    {
                        var auditLog = _auditLogFactory.Create(
                            businessId,
                            TableNames.InventoryCount,
                            result.Id.Value,
                            userId);

                        await _auditService.RegisterCreateAsync(
                            _mapper.Map<Core.Entities.Logistic.InventoryCount>(created.Header),
                            auditLog,
                            transaction);
                    }
                }

                transaction.Commit();
                return result;
            }
            catch
            {
                TryRollback(transaction);
                throw;
            }
        }

        private static void TryRollback(System.Data.IDbTransaction transaction)
        {
            try { transaction.Rollback(); } catch { }
        }
    }
}
