using Core.Entities.Operations;
using Core.Entities.paginations;
using SharedKernel;
using System.Data;

namespace Core.Interfaces.Operations
{
    public interface IOperationsWorkOrderRepository
    {
        Task<PagedResult<OperationWorkOrder>> GetAllAsync(long businessId, int page, int pageSize, string? search, long? operationsId);
        Task<OperationWorkOrder?> GetByIdAsync(long workOrderId, long businessId);
        Task<OperationWorkOrder?> GetByIdAsync(long workOrderId, long businessId, IDbTransaction transaction);
        Task<BaseResponseId> CreateAsync(OperationWorkOrder entity, long userId, long businessId, IDbTransaction transaction);
        Task<BaseResponse> UpdateAsync(OperationWorkOrder entity, IDbTransaction transaction);
        Task<BaseResponse> DeleteAsync(long workOrderId, long businessId, long userId, IDbTransaction transaction);
    }
}