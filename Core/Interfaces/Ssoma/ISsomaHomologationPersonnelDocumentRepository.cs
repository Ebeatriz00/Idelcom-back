using Core.Entities.paginations;
using Core.Entities.Ssoma;
using SharedKernel;
using System.Data;

namespace Core.Interfaces.Ssoma
{
    public interface ISsomaHomologationPersonnelDocumentRepository
    {
        Task<PagedResult<SsomaHomologationPersonnelDocument>> GetAllAsync(
            long businessId,
            long? homologationPersonnelId,
            int? requirementId,
            int page,
            int pageSize,
            string? search);
        Task<SsomaHomologationPersonnelDocument?> GetByIdAsync(long id, long businessId);
        Task<SsomaHomologationPersonnelDocument?> GetByIdAsync(long id, long businessId, IDbTransaction transaction);
        Task<SsomaHomologationPersonnelDocument?> GetActiveByHomologationAndRequirementAsync(
            long businessId,
            long homologationPersonnelId,
            int requirementId,
            IDbTransaction transaction);
        Task<BaseResponseId> CreateAsync(SsomaHomologationPersonnelDocument entity, IDbTransaction transaction);
        Task<BaseResponse> UpdateAsync(SsomaHomologationPersonnelDocument entity, IDbTransaction transaction);
        Task<BaseResponse> MarkAsReplacedAsync(
            long id,
            long businessId,
            long replacedDocumentId,
            string? replacementReason,
            long userId,
            IDbTransaction transaction);
        Task<BaseResponse> DeleteAsync(long id, long businessId, long userId, IDbTransaction transaction);
    }
}
