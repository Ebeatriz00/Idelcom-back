using Core.Entities.Operations.Core.Entities;
using Core.Entities.paginations;
using SharedKernel;
using System.Data;

namespace Core.Interfaces.Operations
{
    public interface IOperationsWorkOrderResponsibleRepository
    {
        Task<PagedResult<OperationWorkOrderResponsible>> GetAllAsync(long businessId, int page, int pageSize, string? search);
        Task<OperationWorkOrderResponsible?> GetByIdAsync(long workOrderResponsibleId, long businessId);
        Task<OperationWorkOrderResponsible?> GetByIdAsync(long workOrderResponsibleId, long businessId, IDbTransaction transaction);
        Task<BaseResponseId> CreateAsync(OperationWorkOrderResponsible entity, long userId, long businessId, IDbTransaction transaction);
        Task<BaseResponse> UpdateAsync(OperationWorkOrderResponsible entity, IDbTransaction transaction);
        Task<BaseResponse> DeleteAsync(long workOrderResponsibleId, long businessId, long userId, IDbTransaction transaction);
    }
}
