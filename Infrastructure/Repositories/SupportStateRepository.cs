using Core.Entities;
using Core.Entities.paginations;
using Core.Interfaces;
using Core.Projections;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories
{
    public class SupportStateRepository(IDapperHelper dapperHelper) : ISupportStateRepository
    {
        private readonly IDapperHelper _dapperHelper = dapperHelper;

        public async Task<PagedSelect<SupportStateSelectItem?>> GetForSelectAsync(long businessId, int page, int pageSize, string search)
        {
            var parameters = DapperParams.From(new
            {
                BusinessId = businessId,
                Search = search,
                PageNumber = page,
                PageSize = pageSize
            });

            var result = await _dapperHelper.QueryAsync<SupportStateSelectItem>("SP_WS_SELECT_SUPPORT_STATE", parameters);
            return new PagedSelect<SupportStateSelectItem?>
            {
                Items = result.ToList()!,
                Page = page,
                PageSize = pageSize,
            };
        }

        public async Task<SupportState?> GetByIdAsync(int supportStateId, long businessId)
        {
            var parameters = DapperParams.From(new
            {
                SupportStateId = supportStateId,
                BusinessId = businessId
            });

            return await _dapperHelper.QueryFirstOrDefaultAsync<SupportState>("SP_WS_GETBYID_SUPPORT_STATE", parameters);
        }
    }
}
