using Core.Entities.Operations;
using Core.Projections.Operations;
using Core.Results.Operations;
using SharedKernel;
using System.Data;

namespace Core.Interfaces.Operations
{
    public interface IOperationsTeamSsomaRepository
    {
        Task<OperationsTeamSsomaCreateResult> AddAsync(
            OperationsTeamSsoma entity,
            IEnumerable<OperationsTeamSsomaAssignmentItem> teamSsoma,
            IDbTransaction transaction);

        Task<OperationsTeamSsomaDetailProjection?> GetByIdAsync(long operationsTeamSsomaId);
        Task<OperationsTeamSsomaDetailProjection?> GetByIdAsync(long operationsTeamSsomaId, IDbTransaction transaction);

        Task<GlobalResponse> UpdateAsync(
            OperationsTeamSsoma entity,
            IEnumerable<OperationsTeamSsomaAssignmentItem> teamSsoma,
            IDbTransaction transaction);

        Task<List<long>> GetExistingWorkerIdsAsync(
            long businessId,
            long ssomaProcessId,
            IEnumerable<long> workerIds,
            IDbTransaction transaction);

        Task<OperationsTeamSsomaDetailProjection?> GetByProcessIdAsync(
            long businessId,
            long ssomaProcessId);

        Task<OperationsTeamSsomaDetailProjection?> GetByProcessIdAsync(
            long businessId,
            long ssomaProcessId,
            IDbTransaction transaction);

        Task<IEnumerable<OperationsTeamSsomaListItemProjection>> GetListByProcessIdAsync(
            long businessId,
            long ssomaProcessId);

        Task<GlobalResponse> UpdateTeamSssomaAssignmentStatusAsync(OperationsTeamSsoma entity, IDbTransaction transaction);
        Task<GlobalResponse> UpdateReplacedAssignmentIdAsync(
            long businessId,
            long operationsTeamSsomaId,
            long replacedAssignmentId,
            long updateUser,
            IDbTransaction transaction);
        Task<GlobalResponse> RelocationTeamSsomaAsync(OperationsTeamSsoma entity, IDbTransaction transaction);
        Task<GlobalResponse> ReplacementTeamSsomaAsync(OperationsTeamSsoma entity, IDbTransaction transaction);

        Task<ActiveSsomaAssignmentProjection?> GetActiveAssignmentByWorkerIdAsync(
            long businessId,
            long workerId);

        Task<GlobalResponse> DeleteAsync(
            long operationsTeamSsomaId, 
            long userId, 
            long businessId, 
            IDbTransaction? transaction = null);
    }
}
