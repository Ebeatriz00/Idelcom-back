using Application.DTOs.InventoryCount;
using Application.Exceptions;
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
    public class UpdateInventoryCountDetailsUseCase(
        IInventoryCountRepository repository,
        IValidator<InventoryCountUpdateDetailsDto> validator,
        ISqlConnectionFactory sqlConnectionFactory,
        IAuditService auditService,
        IAuditLogFactory auditLogFactory,
        IMapper mapper)
    {
        private readonly IInventoryCountRepository _repository = repository;
        private readonly IValidator<InventoryCountUpdateDetailsDto> _validator = validator;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;
        private readonly IAuditService _auditService = auditService;
        private readonly IAuditLogFactory _auditLogFactory = auditLogFactory;
        private readonly IMapper _mapper = mapper;

        public async Task<BaseResponse> ExecuteAsync(InventoryCountUpdateDetailsDto dto, long userId, long businessId)
        {
            await InventoryCountValidation.ValidateAsync(_validator, dto);

            using var connection = _sqlConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var before = await _repository.GetByIdAsync(businessId, dto.InventoryCountId, transaction);
                if (before?.Header is null)
                    throw new NotFoundException("Toma de inventario", dto.InventoryCountId);

                var command = _mapper.Map<InventoryCountUpdateDetailsCommand>(dto);
                command.BusinessId = businessId;
                command.UserId = userId;

                var result = await _repository.UpdateDetailsAsync(command, transaction);

                var after = await _repository.GetByIdAsync(businessId, dto.InventoryCountId, transaction);
                if (after?.Header is null)
                    throw new NotFoundException("Toma de inventario", dto.InventoryCountId);

                var updatedDetailIds = dto.Details.Select(d => d.InventoryCountDetailId).ToHashSet();

                foreach (var beforeDetail in _mapper.Map<List<InventoryCountDetail>>(before.Details.Where(d => updatedDetailIds.Contains(d.InventoryCountDetailId))))
                {
                    beforeDetail.BusinessId = businessId;
                    var afterDetail = after.Details.FirstOrDefault(d => d.InventoryCountDetailId == beforeDetail.InventoryCountDetailId);
                    if (afterDetail is null) continue;

                    var afterEntity = _mapper.Map<InventoryCountDetail>(afterDetail);
                    afterEntity.BusinessId = businessId;

                    var detailAuditLog = _auditLogFactory.Create(
                        businessId,
                        TableNames.InventoryCountDetail,
                        beforeDetail.InventoryCountDetailId,
                        userId,
                        "Actualizacion de conteo");

                    await _auditService.RegisterUpdateAsync(beforeDetail, afterEntity, detailAuditLog, transaction);
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
