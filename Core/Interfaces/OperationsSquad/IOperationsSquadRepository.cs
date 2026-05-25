using SharedKernel;
using System;
using Core.Entities.OperationsSquad;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities.paginations;

namespace Core.Interfaces.OperationsSquad
{
    public interface IOperationsSquadRepository
    {
        Task<BaseResponseId> CreateAsync(OperationSquad entity, IDbTransaction transaction);
        Task<PagedResult<OperationSquad>> GetAllAsync(long businessId, string? search, int page, int pageSize, long? workOrderId);
        Task<OperationSquad?> GetByIdAsync(long squadId);
        Task<BaseResponse> UpdateAsync(OperationSquad entity, IDbTransaction transaction);
        Task<BaseResponse> DeleteAsync(long squadId, long userId, IDbTransaction transaction);
    }
}
