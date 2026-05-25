using Core.Entities.Operations;
using Core.Entities.paginations;
using Core.Projections.Operations;
using SharedKernel;
using System.Data;

namespace Core.Interfaces.Operations
{
    public interface IOperationsRepository
    {
        Task<PagedResult<OperationsListItemProjection>> GetAllAsync(long businessId, int page, int pageSize);
        Task<OperationsListItemProjection?> GetByIdAsync(long operationsId);
        Task<BaseResponseId> CreateAsync(Operation entity, long userId, long businessId, IDbTransaction transaction);
        Task<BaseResponse> UpdateAsync(Operation entity, long userId, long businessId, IDbTransaction transaction);
        Task<BaseResponse> DeleteAsync(long operationsId, long userId, IDbTransaction transaction);
        Task<DateTime?> GetOperationEndDateAsync(long operationsId, IDbTransaction? transaction = null);
    }
}
