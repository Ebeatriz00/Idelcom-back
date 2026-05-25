using Core.Entities.Logistic;
using Core.Entities.paginations;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Logistic
{
    public interface IBrandsRepository
    {
        Task<bool> ExistsAsync(string description, long businessId, long? excludeId = null);
        Task<BaseResponseId> AddAsync(Brands entity, IDbTransaction transaction);
        Task<PagedResult<Brands>> GetAllAsync(long businessId, string? search, int page, int pageSize);
        Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, string? search, int page, int pageSize);
        Task<Brands?> GetByIdAsync(long brandsId);
        Task<Brands?> GetByIdAsync(long brandsId, IDbTransaction transaction);
        Task<BaseResponse> UpdateAsync(Brands entity, IDbTransaction transaction);
        Task<bool> PatchStatusAsync(long brandsId, string status, long userId, long businessId);
    }
}
