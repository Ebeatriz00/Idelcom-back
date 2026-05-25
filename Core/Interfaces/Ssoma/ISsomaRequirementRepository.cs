using Core.Entities.Operations;
using Core.Entities.paginations;
using Core.Entities.Ssoma;
using Core.Projections.SsomRequirement;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Ssoma
{
    public interface ISsomaRequirementRepository
    {
        Task<BaseResponseId> CreateAsync(SsomaRequirement entity, IDbTransaction transaction);
        Task<BaseResponse> UpdateAsync(SsomaRequirement entity, IDbTransaction transaction);
        Task<BaseResponse> DeleteAsync(long id, long businessId, long userId, IDbTransaction transaction);
        Task<PagedResult<SsomaRequirementItem>> GetAllItemAsync( long businessId, int page, int pageSize, string? search);
        Task<SsomaRequirement?> GetByIdAsync(long id, long businessId);
        Task<SsomaRequirement?> GetByIdAsync(long id, long businessId, IDbTransaction transaction);
        Task<PagedResult<SsomaRequirement>> GetAllAsync(long businessId, int scopeId, int page, int pageSize, string? search);
        Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, int scopedId, string? search, int page, int pageSize);

    }
}
