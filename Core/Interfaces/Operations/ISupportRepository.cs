using SharedKernel;
using Core.Entities.Operations;
using System.Data;
using Core.Entities.paginations;

namespace Core.Interfaces.Operations
{
    public interface ISupportRepository
    {
        Task<BaseResponseId> CreateAsync(Support entity, IDbTransaction transaction);
        Task<PagedResult<Support>> GetAllAsync(long businessId, string? search, int page, int pageSize);
        Task<Support?> GetByIdAsync(long supportId, long businessId);
        Task<BaseResponse> UpdateAsync(Support entity, IDbTransaction transaction);
        Task<BaseResponse> DeleteAsync(long supportId, long userId, long businessId, IDbTransaction transaction);
    }
}
