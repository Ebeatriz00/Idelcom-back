using Core.Entities.paginations;
using Core.Interfaces;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;
using Microsoft.Data.SqlClient;

namespace Infrastructure.Repositories
{
    public class SupplierInvoiceStatusRepository(IDapperHelper dapperHelper) : ISupplierInvoiceStatusRepository
    {
        private readonly IDapperHelper _dapperHelper = dapperHelper;

        public async Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, string? search, int page, int pageSize)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    BusinessId = businessId,
                    Search = search,
                    PageNumber = page,
                    PageSize = pageSize
                });

                var (items, total) = await _dapperHelper.QueryPagedAsync<OptionItem>("SP_WS_SUPPLIER_INVOICE_STATUS_SELECT", parameters);

                return new PagedSelect<OptionItem>
                {
                    Items = items.ToList(),
                    Page = page,
                    PageSize = pageSize,
                    HasMore = false
                };
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener los estados de factura de proveedor para el selector.", ex.Message);
            }
        }
    }
}
