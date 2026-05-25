using Core.Entities.Operations;
using Core.Entities.paginations;
using Core.Interfaces.Operations;
using Core.Projections.Operations;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories.Operations
{
    public class MovementStatusRepository(IDapperHelper dapperHelper) : IMovementStatusRepository
    {
        private readonly IDapperHelper _dapperHelper = dapperHelper;

        public async Task<MovementStatus?> GetByIdAsync(long movementStatusId, long businessId)
        {
            var parameters = DapperParams.From(new
            {
                MovementStatusId = movementStatusId,
                BusinessId = businessId
            });

            return await _dapperHelper.QueryFirstOrDefaultAsync<MovementStatus>("SP_WS_GETBYID_MOVEMENT_STATUS", parameters);
        }

        public async Task<PagedSelect<MovementStatusSelectItem>> GetSelectAsync(long businessId, int page, int pageSize, string? search)
        {
            var parameters = DapperParams.From(new
            {
                BusinessId = businessId,
                Page = page,
                PageSize = pageSize,
                Search = search
            });

            var result = await _dapperHelper.QueryAsync<MovementStatusSelectItem>("SP_WS_SELECT_MOVEMENT_STATUS", parameters);

            return new PagedSelect<MovementStatusSelectItem>
            {
                Items = result.ToList(),
                Page = page,
                PageSize = pageSize,
                HasMore = false
            };
        }
    }
}
