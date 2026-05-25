using Core.Entities.OperationsPersonnelAssignment;
using Core.Entities.paginations;
using Core.Projections.OperationsPersonnelAssignment;
using SharedKernel;
using System.Data;
using System.Threading.Tasks;

namespace Core.Interfaces.OperationsPersonnelAssignment
{
    public interface IOperationsPersonnelAssignmentRepository
    {
        Task<BaseResponseId> CreateAsync(OperationPersonnelAssignment entity, IDbTransaction transaction);
        Task<PagedResult<OperationPersonnelAssignmentProjection>> GetAllAsync(long businessId, string? search, int page, int pageSize);
        Task<BaseResponse> UpdateAsync(OperationPersonnelAssignment entity, IDbTransaction transaction);
        Task<BaseResponse> DeleteAsync(long assignmentId, long userId, IDbTransaction transaction);
        Task<OperationPersonnelAssignment?> GetByIdAsync(long assignmentId);
    }
}
