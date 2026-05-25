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
    public interface IWarehousesRepository
    {
        Task<bool> ExistsAsync(string description, long businessId, long? excludeId = null);
        Task<BaseResponseId> AddAsync(Warehouses entity, IDbTransaction transaction);
        Task<PagedResult<WarehousesItem>> GetAllAsync(long businessId, string? search, int page, int pageSize);
        Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, string? search, int page, int pageSize);
        Task<Warehouses?> GetByIdAsync(long warehousesId);
        Task<Warehouses?> GetByIdAsync(long warehousesId, IDbTransaction transaction);
        Task<BaseResponse> UpdateAsync(Warehouses entity, IDbTransaction transaction);
        Task<bool> PatchStatusAsync(long warehousesId, string status, long userId, long businessId);
    }
}
