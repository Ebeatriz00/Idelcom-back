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
    public interface IProductsRepository
    {
        Task<bool> ExistsAsync(string description, long businessId, long? excludeId = null);
        Task<BaseResponseId> AddAsync(Products entity, IDbTransaction transaction);
        Task<PagedResult<ProductsItem>> GetAllAsync(long businessId, long? categoriesId, long? productTypeId, long? brandsId, string? search, int page, int pageSize);
        Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId,  string? search, int page, int pageSize);
        Task<Products?> GetByIdAsync(long productsId);
        Task<Products?> GetByIdAsync(long productsId, IDbTransaction transaction);
        Task<BaseResponse> UpdateAsync(Products products, IDbTransaction transaction);
        Task<bool> PatchStatusAsync(long productsId, string status, long userId, long businessId);


    }
}
