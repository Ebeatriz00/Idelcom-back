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

namespace Core.Interfaces.Logistic
{
    public interface IProductTypesRepository
    {
        Task<bool> ExistsAsync(string description, long businessId, long? excludeId = null);
        Task<BaseResponseId> AddAsync(ProductTypes entity,  IDbTransaction transaction);
        Task<PagedResult<ProductTypeItem>> GetAllAsync(long businessId, string? search, int page, int pageSize);
        Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, string? search, int page, int pageSize);
        Task<ProductTypes?> GetByIdAsync(long productTypesId);
        Task<ProductTypes?> GetByIdAsync(long productTypesId, IDbTransaction transaction);
        Task<BaseResponse> UpdateAsync(ProductTypes entity, IDbTransaction transaction);
        Task<bool> PatchStatusAsync(long productTypesId, string status, long createUser, long businessId);
    }
}
