using Core.Entities.paginations;

namespace Core.Interfaces
{
    public interface IPurchaseReceiptStatusRepository
    {
        Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, string? search, int page, int pageSize);
    }
}
