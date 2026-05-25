using Application.DTOs.PurchaseOrder;
using AutoMapper;
using Core.Commands.Logistic;
using Core.Entities.Logistic;
using Core.Interfaces.Audit;
using Core.Interfaces.Logistic;
using FluentValidation;
using Infrastructure.Persistence;
using SharedKernel;
using SharedKernel.Constants;

namespace Application.UseCases.PurchaseOrder
{
    public class RegisterPurchaseOrder(
        IPurchaseOrderRepository repository,
        IValidator<PurchaseOrderCreateDto> validator,
        ISqlConnectionFactory sqlConnectionFactory,
        IAuditService auditService,
        IAuditLogFactory auditLogFactory,
        IMapper mapper)
    {
        private readonly IPurchaseOrderRepository _repository = repository;
        private readonly IValidator<PurchaseOrderCreateDto> _validator = validator;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;
        private readonly IAuditService _auditService = auditService;
        private readonly IAuditLogFactory _auditLogFactory = auditLogFactory;
        private readonly IMapper _mapper = mapper;

        public async Task<BaseResponseId> ExecuteAsync(PurchaseOrderCreateDto dto, long userId, long businessId)
        {
            await PurchaseOrderValidation.ValidateAsync(_validator, dto);

            using var connection = _sqlConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var command = _mapper.Map<PurchaseOrderCommand>(dto);
                command.BusinessId = businessId;
                command.UserId = userId;

                var result = await _repository.RegisterAsync(command, transaction);
                if (result.Id is > 0)
                {
                    var created = await _repository.GetByIdAsync(businessId, result.Id.Value, transaction);
                    if (created?.Header is not null)
                    {
                        var auditLog = _auditLogFactory.Create(
                            businessId,
                            TableNames.PurchaseOrder,
                            result.Id.Value,
                            userId);

                        await _auditService.RegisterCreateAsync(
                            _mapper.Map<Core.Entities.Logistic.PurchaseOrder>(created.Header),
                            auditLog,
                            transaction);

                        foreach (var detail in MapDetailEntities(created))
                        {
                            var detailAuditLog = _auditLogFactory.Create(
                                businessId,
                                TableNames.PurchaseOrderDetail,
                                detail.PurchaseOrderDetailId,
                                userId);

                            await _auditService.RegisterCreateAsync(
                                detail,
                                detailAuditLog,
                                transaction);
                        }
                    }
                }

                transaction.Commit();
                return result;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        private List<PurchaseOrderDetail> MapDetailEntities(Core.Projections.Logistic.PurchaseOrderByIdProjection source)
        {
            var details = _mapper.Map<List<PurchaseOrderDetail>>(source.Details);
            foreach (var detail in details)
            {
                detail.BusinessId = source.Header?.BusinessId ?? 0;
                detail.PurchaseOrderId = source.Header?.PurchaseOrderId ?? 0;
            }

            return details;
        }
    }
}
