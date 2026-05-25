using Core.Entities.paginations;
using Core.Projections.Ssoma;

namespace Core.Interfaces.Ssoma
{
    public interface ISsomaRoleRepository
    {
        Task<PagedSelect<SsomaRoleSelectItem?>> GetForSelectAsync(long businessId, int page, int pageSize, string? search);
    }
}
