using Core.Entities.paginations;
using Core.Projections.Operations;

namespace Core.Interfaces.Operations
{
    public interface IActivityComplexityRepository
    {
        Task<PagedSelect<ActivityComplexitySelectItem?>> GetForSelectAsync(long businessId, int page, int pageSize, string? search);
    }
}
