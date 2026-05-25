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
    public interface ISuppliersRepository
    {
        Task<bool> ExistsAsync(string documentNumber,string supplierName, long businessId, long? excludeId = null);
        Task<BaseResponseId> AddAsync(Suppliers entity, IDbTransaction transaction);
        Task<PagedResult<SupplierItem>> GetAllAsync(long businessId, string? search, int page, int pageSize);
        Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, string? search, int page, int pageSize);
        Task<Suppliers?> GetByIdAsync(long suppliersId);
        Task<Suppliers?> GetByIdAsync(long suppliersId, IDbTransaction transaction);
        Task<BaseResponse> UpdateAsync(Suppliers entity, IDbTransaction transaction);
        Task<bool> PatchStatusAsync(long suppliersId, string status, long userId, long businessId);
    }
}
