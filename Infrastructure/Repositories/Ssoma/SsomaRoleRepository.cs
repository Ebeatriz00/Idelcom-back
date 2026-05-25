using Core.Entities.paginations;
using Core.Interfaces.Ssoma;
using Core.Projections.Ssoma;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories.Ssoma
{
    public class SsomaRoleRepository(IDapperHelper dapperHelper) : ISsomaRoleRepository
    {
        private readonly IDapperHelper _dapperHelper = dapperHelper;

        public async Task<PagedSelect<SsomaRoleSelectItem?>> GetForSelectAsync(long businessId, int page, int pageSize, string? search)
        {
            var parameters = DapperParams.From(new
            {
                BusinessId = businessId,
                Page = page,
                PageSize = pageSize,
                Search = search
            });

            var result = await _dapperHelper.QueryAsync<SsomaRoleSelectItem>("SP_WS_SELECT_SSOMA_ROLE", parameters);

            return new PagedSelect<SsomaRoleSelectItem?>
            {
                Items = result.ToList(),
                Page = page,
                PageSize = pageSize,
                HasMore = false
            };
        }
    }
}
