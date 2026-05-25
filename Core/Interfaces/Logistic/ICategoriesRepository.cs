using Core.Entities.Logistic;
using Core.Entities.paginations;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.logistic
{
    public interface ICategoriesRepository
    {
        Task<bool> ExistsAsync(string description, long businessId, long? excludeId = null);
        Task<BaseResponseId> AddAsync(Categories entity, IDbTransaction transaction);
        Task<PagedResult<Categories>> GetAllAsync(long businessId, string? search, int page, int pageSize);
        Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, string? search, int page, int pageSize);
        Task<Categories?> GetByIdAsync(long categoriesId);
        Task<Categories?> GetByIdAsync(long categoriesId, IDbTransaction transaction);
        Task<BaseResponse> UpdateAsync(Categories entity, IDbTransaction transaction);
        Task<bool> PatchStatusAsync(long categoriesId, string status, long userId, long businessId);
    }
}
