using Core.Entities;
using Core.Entities.paginations;
using Core.Projections;

namespace Core.Interfaces
{
    public interface ISupportStateRepository
    {
        Task<PagedSelect<SupportStateSelectItem?>> GetForSelectAsync(long businessId, int page, int pageSize, string search);
        Task<SupportState?> GetByIdAsync(int supportStateId, long businessId);
    }
}
