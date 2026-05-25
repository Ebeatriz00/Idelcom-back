using Core.Entities.Operations;
using Core.Entities.paginations;
using Core.Interfaces.Operations;
using Core.Projections.Operations;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories.Operations
{
    public class OperationsWorkOrderStatusRepository(IDapperHelper dapperHelper) : IOperationsWorkOrderStatusRepository
    {
        private readonly IDapperHelper _dapperHelper = dapperHelper;

        public async Task<PagedSelect<OperationsWorkOrderStatusSelectItem?>> GetForSelectAsync(long businessId, int page, int pageSize, string? search)
        {
            var parameters = DapperParams.From(new
            {
                BusinessId = businessId,
                PageNumber = page,
                PageSize = pageSize,
                Search = search
            });

            var result = await _dapperHelper.QueryAsync<OperationsWorkOrderStatusSelectItem>("SP_WS_SELECT_WORK_ORDER_STATUS", parameters);

            return new PagedSelect<OperationsWorkOrderStatusSelectItem?>
            {
                Items = result.ToList(),
                Page = page,
                PageSize = pageSize,
                HasMore = false
            };
        }

        public async Task<OperationWorkOrderStatus?> GetByIdAsync(long workOrderStatusId, long businessId)
        {
            var parameters = DapperParams.From(new
            {
                BusinessId = businessId,
                WorkOrderStatusId = workOrderStatusId
            });

            return await _dapperHelper.QueryFirstOrDefaultAsync<OperationWorkOrderStatus>("SP_WS_GETBYID_WORK_ORDER_STATUS", parameters);
        }

    }

}
