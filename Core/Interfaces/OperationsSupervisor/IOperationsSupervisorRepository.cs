using Core.Entities.OperationsSupervisor;
using Core.Entities.paginations;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.OperationsSupervisor
{
    public interface IOperationsSupervisorRepository
    {
        Task<BaseResponseId> CreateAsync(OperationSupervisor entity, long businessId, IDbTransaction transaction);
        Task<PagedResult<OperationSupervisor>> GetAllAsync(long businessId, string? search, int page, int pageSize);
        Task<BaseResponse> UpdateAsync(OperationSupervisor entity, long businessId, IDbTransaction transaction);
        Task<BaseResponse> DeleteAsync(long supervisorId, long userId, IDbTransaction transaction);
        Task<OperationSupervisor?> GetByIdAsync(long supervisorId);
    }
}
