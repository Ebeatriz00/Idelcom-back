using Core.Entities.Operations;
using Core.Entities.paginations;
using SharedKernel;
using System.Data;

namespace Core.Interfaces.Operations
{
    public interface IOperationsProjectConfigRepository
    {
        Task<GlobalResponse> AddAsync(OperationsProjectConfig entity,  IDbTransaction transaction);
        Task<IEnumerable<OperationsProjectConfig>> GetAllAsync(long operationsId);
        Task<OperationsProjectConfig?> GetByIdAsync(long operationsProjectConfigId, long operationsId);
        Task<OperationsProjectConfig?> GetByIdAsync(long operationsProjectConfigId, long operationsId, IDbTransaction transaction);
        Task<GlobalResponse> UpdateAsync(OperationsProjectConfig entity, IDbTransaction transaction);
    }
}
