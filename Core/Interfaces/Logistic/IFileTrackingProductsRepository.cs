using Core.Entities;
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
    public interface IFileTrackingProductsRepository
    {
        Task<bool> ExistsAsync(long businessId, long productsId, string fileName, long? excludeId = null);
        Task<BaseResponseId> AddAsync(FileTrackingProducts entities, IDbTransaction transaction);
        Task<PagedResult<FileTrackingProducts>> GetAllAsync(long businessId, long productsId, int page, int pageSize);
        Task<BaseResponse> DeleteAsync(long fileId, long productsId, long businessId, IDbTransaction transaction);


    }
}
