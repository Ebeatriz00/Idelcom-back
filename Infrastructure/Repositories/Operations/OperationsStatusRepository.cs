using Core.Entities.Operations;
using Core.Entities.paginations;
using Core.Interfaces.Operations;
using Core.Projections.Operations;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories.Operations
{
    public class OperationsStatusRepository(IDapperHelper dapperHelper) : IOperationsStatusRepository
    {
        private readonly IDapperHelper _dapperHelper = dapperHelper;

        public async Task<PagedSelect<OperationsStatusSelectItem>> GetForSelectAsync(long businessId, int page, int pageSize, string? search)
        {
            var parameters = DapperParams.From(new
            {
                BusinessId = businessId,
                PageNumber = page,
                PageSize = pageSize,
                Search = search
            });

            var (items, total) = await _dapperHelper.QueryPagedAsync<OperationsStatusSelectItem>("SP_WS_SELECT_OPERATIONS_STATUS", parameters);

            return new PagedSelect<OperationsStatusSelectItem>
            {
                Items = items.ToList(),
                Page = page,
                PageSize = pageSize,
                HasMore = false
            };
        }

        public async Task<OperationsStatus?> GetByIdAsync(long operationsStatusId, long businessId)
        {
            var parameters = DapperParams.From(new
            {
                BusinessId = businessId,
                OperationsStatusId = operationsStatusId
            });

            return await _dapperHelper.QueryFirstOrDefaultAsync<OperationsStatus>("SP_WS_GETBYID_OPERATIONS_STATUS", parameters);
        }


    }
}
