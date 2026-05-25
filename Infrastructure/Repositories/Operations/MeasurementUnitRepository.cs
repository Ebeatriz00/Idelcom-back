using Core.Entities.paginations;
using Core.Interfaces.Operations;
using Core.Projections.Operations;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories.Operations
{
    public class MeasurementUnitRepository(IDapperHelper dapperHelper) : IMeasurementUnitRepository
    {
        private readonly IDapperHelper _dapperHelper = dapperHelper;

        public async Task<PagedSelect<MeasurementUnitSelectItem?>> GetForSelectAsync(long businessId, int page, int pageSize, string? search)
        {
            var parameters = DapperParams.From(new
            {
                BusinessId = businessId,
                Page = page,
                PageSize = pageSize,
                Search = search
            });

            var result = await _dapperHelper.QueryAsync<MeasurementUnitSelectItem>("SP_WS_SELECT_MEASUREMENT_UNIT", parameters);

            return new PagedSelect<MeasurementUnitSelectItem?>
            {
                Items = result.ToList(),
                Page = page,
                PageSize = pageSize,
                HasMore = false
            };
        }
    }
}
