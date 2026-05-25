using Core.Entities.paginations;
using Core.Interfaces.Operations;
using Core.Projections.Operations;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories.Operations
{
    public class ActivityComplexityRepository(IDapperHelper dapperHelper) : IActivityComplexityRepository
    {
        private readonly IDapperHelper _dapperHelper = dapperHelper;

        public async Task<PagedSelect<ActivityComplexitySelectItem?>> GetForSelectAsync(long businessId, int page, int pageSize, string? search)
        {
            var parameters = DapperParams.From(new
            {
                BusinessId = businessId,
                Page = page,
                PageSize = pageSize,
                Search = search
            });

            var result = await _dapperHelper.QueryAsync<ActivityComplexitySelectItem>("SP_WS_SELECT_ACTIVITY_COMPLEXITY", parameters);

            return new PagedSelect<ActivityComplexitySelectItem?>
            {
                Items = result.ToList(),
                Page = page,
                PageSize = pageSize,
                HasMore = false
            };
        }
    }
}
