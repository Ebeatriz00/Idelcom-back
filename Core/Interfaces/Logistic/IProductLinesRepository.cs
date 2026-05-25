using Core.Entities.Logistic;
using Core.Entities.paginations;
using Core.Projections.Logistic;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.logistic
{
    public interface IProductLinesRepository
    {
        Task<bool> ExistsAsync(string description, long businessId, long? excludeId = null);
        Task<BaseResponseId> AddAsync(ProductLines entity, IDbTransaction transaction);
        Task<PagedResult<ProductLinesItem>> GetAllAsync(long businessId, string? search, int page, int pageSize);
        Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId,long? categoriesId, string? search, int page, int pageSize);
        Task<ProductLines?> GetByIdAsync(long productLinesId);
        Task<ProductLines?> GetByIdAsync(long productLinesId, IDbTransaction transaction);
        Task<BaseResponse> UpdateAsync(ProductLines entity, IDbTransaction transaction);
        Task<bool> PatchStatusAsync(long productLinesId, string status, long userId, long businessId);
   
    }
}

