
using Core.Entities.paginations;
using Core.Entities.Ssoma;
using Core.Projections.Ssoma;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Ssoma
{
    public interface ISsomaProcessRepository
    {
        Task<GlobalResponse> AddAsync(SsomaProcess entity,  IDbTransaction transaction);
        Task<GlobalResponse> UpdateAsync(SsomaProcess entity,  IDbTransaction transaction);
        Task<GlobalResponse> DeleteAsync(long ssomaProcessId, long userId, IDbTransaction transaction);
        Task<PagedResult<SsomaProcessListItem>> GetAllAsync(long businessId, int page, int pageSize, string? search, long? operationsId = null);
        Task<SsomaProcess?> GetByIdAsync(long ssomaProcessId, long operationsId, long businessId);
        Task<SsomaProcess?> GetByIdAsync(long ssomaProcessId, long operationsId, long businessId, IDbTransaction transaction);

    }
}
