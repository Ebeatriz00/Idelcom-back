using Core.Entities;
using Core.Entities.paginations;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ISalesQuotationRepository
    {
        Task<GlobalResponse> AddAsync(SalesQuotation entity, CancellationToken ct = default);
        Task<GlobalResponse> AddVerAsync(SalesQuotation entity, CancellationToken ct = default);
        Task<PagedResult<SalesQuotation>> GetAllAsync(long businessId, string? search, int page, int pageSize, long? usersId, string? verDesc, long? workerId);
        Task<PagedResult<SalesQuotation>> GetAllVerAsync(string quotationId ,long businessId, string? search, int page, int pageSize, string? verDesc, long? workerId, long? workerResponsibles);
        Task<SalesQuotation?> GetDetailAsync(long quotationVerId, long businessId, string? versionNo, CancellationToken ct = default);

    }
}
