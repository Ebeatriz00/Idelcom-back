using Core.Entities.Operations;
using Core.Entities.paginations;
using Core.Projections.Operations;
using SharedKernel;
using System.Data;

namespace Core.Interfaces.Operations
{
    public interface IOperationsWorkOrderActivityRepository
    {
        Task<BaseResponseId> CreateAsync(OperationWorkOrderActivity entity, long userId, long businessId, IDbTransaction transaction);
        Task<OperationWorkOrderActivity?> GetByIdAsync(long activityId, long businessId);
        Task<OperationWorkOrderActivity?> GetByIdAsync(long activityId, long businessId, IDbTransaction transaction);
        Task<PagedResult<OperationWorkOrderActivity>> GetAllAsync(long workOrderId, long businessId, int page, int pageSize, string? search);
        Task<(IEnumerable<AppOperationProjection> Operations,
              IEnumerable<AppWorkOrderProjection> WorkOrders,
              IEnumerable<AppRootActivityProjection> RootActivities,
              IEnumerable<AppSubActivityProjection> SubActivities)> GetAppActivitiesByResponsibleAsync(long userId, long businessId);
        Task<IEnumerable<OperationWorkOrderActivity>> GetSubActivitiesAsync(long parentId, long businessId);
        Task<BaseResponse> UpdateAsync(OperationWorkOrderActivity entity, long userId, long businessId, IDbTransaction transaction);
        Task<BaseResponse> DeleteAsync(long activityId, long businessId, long userId, IDbTransaction transaction);
        Task<PagedSelect<OperationsWorkOrderActivitySelectItem?>> GetForSelectAsync(long businessId, long operationsId, int page, int pageSize, string? search);
    }
}
