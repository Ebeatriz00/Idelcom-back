using Core.Entities.Operations;
using Core.Entities.paginations;
using Core.Entities.Ssoma;
using Core.Projections.SsomaOperationsRequirement;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Ssoma
{
  
    public interface ISsomaOperationsRequirementRepository
    {
        Task<PagedResult<SsomaOperationsRequirementItem>> GetAllAsync(long businessId, long operationsId, int page, int pageSize, string? search);
        Task<PagedResult<SsomaOperationsRequirementByWorkerItem>> GetListByWorkerAsync(long businessId, long operationsId, long workerId, int page, int pageSize, string? search);
        Task<ValidateSsomaOperationsRequirementByWorkerItem?> ValidateByWorkerAsync(long businessId, long operationsId, long workerId);
        Task<IEnumerable<SsomaOperationsRequirementMissingByWorkerItem>> GetMissingByWorkerAsync(long businessId, long operationsId, long workerId);

        Task<PagedSelect<OptionItem>> GetForSelectOperationsAsync(long businessId, string? search, int page, int pageSize);
        Task<SsomaOperationsRequirement?> GetByIdAsync(long id, long businessId);
        Task<SsomaOperationsRequirement?> GetByIdAsync(long id, long businessId, IDbTransaction transaction);
        Task<BaseResponseId> CreateAsync(SsomaOperationsRequirement entity, IDbTransaction transaction);
        Task<BaseResponse> UpdateAsync(SsomaOperationsRequirement entity, IDbTransaction transaction);
        Task<BaseResponse> DeleteAsync(long id, long businessId, long userId, IDbTransaction transaction);

    }

}
