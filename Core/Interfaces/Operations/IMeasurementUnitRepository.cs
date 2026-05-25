using Core.Entities.paginations;
using Core.Projections.Operations;

namespace Core.Interfaces.Operations
{
    public interface IMeasurementUnitRepository
    {
        Task<PagedSelect<MeasurementUnitSelectItem?>> GetForSelectAsync(long businessId, int page, int pageSize, string? search);
    }
}
