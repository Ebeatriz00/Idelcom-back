using Core.Entities.Operations;
using Core.Entities.paginations;
using SharedKernel;

namespace Core.Interfaces.Operations
{
    public interface IOperationsWorkOrderProgressRepository
    {
        Task<BaseResponseId> CreateAsync(OperationWorkOrderProgress entity, long userId, long businessId);
        Task<(BaseResponseId Response, bool IsDuplicate)> CreateV2Async(OperationWorkOrderProgress entity, long userId, long businessId, string? appRecordId);
        Task<PagedResult<OperationWorkOrderProgress>> GetAllAsync(long businessId, string? search, int page, int pageSize, long? activityId, string? date, long? operationsId);
    }
}
