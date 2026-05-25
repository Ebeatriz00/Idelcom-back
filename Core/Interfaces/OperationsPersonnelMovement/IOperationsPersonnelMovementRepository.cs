using Core.Entities.OperationsPersonnelMovement;
using Core.Entities.paginations;
using Core.Projections.OperationsPersonnelMovement;
using SharedKernel;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Core.Interfaces.OperationsPersonnelMovement
{
    public interface IOperationsPersonnelMovementRepository
    {
        Task<BaseResponseId> CreateAsync(OperationPersonnelMovement entity, IDbTransaction transaction);
        Task<PagedResult<OperationsPersonnelMovementProjection>> GetAllAsync(long businessId, string? search, int page, int pageSize);
    }
}
