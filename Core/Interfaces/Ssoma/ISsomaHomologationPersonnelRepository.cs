using Core.Entities;
using Core.Entities.paginations;
using Core.Entities.Ssoma;
using Core.Projections.Ssoma;
using SharedKernel;
using System.Data;

namespace Core.Interfaces.Ssoma
{
    public interface ISsomaHomologationPersonnelRepository
    {
        Task<PagedResult<SsomaHomologationPersonnel>> GetAllAsync(long businessId, long? operationsId, int page, int pageSize, string? search);
        Task<SsomaHomologationPersonnel?> GetByIdAsync(long id, long businessId);
        Task<SsomaHomologationPersonnel?> GetByIdAsync(long id, long businessId, IDbTransaction transaction);
        Task<SsomaHomologationPersonnel?> GetActiveByBusinessWorkerScopeAsync(
            long businessId,
            long workerId,
            long homologationScopeId,
            long? operationsId,
            IDbTransaction transaction);
        Task<BaseResponseId> CreateAsync(SsomaHomologationPersonnel entity, IDbTransaction transaction);
        Task<BaseResponse> UpdateAsync(SsomaHomologationPersonnel entity, IDbTransaction transaction);
        Task<BaseResponse> DeleteAsync(long id, long businessId, long userId, IDbTransaction transaction);
        Task<PagedResult<SsomaPersonnelOperationsListItem>> GetListAllPersonnelOperations(long businessId, int page, int pageSize, string? search);
        Task<SsomaPersonnelOperationsItem?> GetDetailPersonnelOperations(long personnelOperationsId, long businessId);
    }
}
