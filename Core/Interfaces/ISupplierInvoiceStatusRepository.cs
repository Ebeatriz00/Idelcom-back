using Core.Entities.paginations;

namespace Core.Interfaces
{
    public interface ISupplierInvoiceStatusRepository
    {
        Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, string? search, int page, int pageSize);
    }
}
