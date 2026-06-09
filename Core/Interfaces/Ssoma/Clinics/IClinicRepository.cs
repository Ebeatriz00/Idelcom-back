using Core.Entities.paginations;
using Core.Entities.Ssoma.Clinics;
using Core.Projections.Ssoma.Clinics;
using SharedKernel;
using System.Data;

namespace Core.Interfaces.Ssoma.Clinics
{
    public interface IClinicRepository
    {
        Task<BaseResponseId> CreateAsync(Clinic entity, IDbTransaction transaction);
        Task<PagedResult<Clinic>> GetAllAsync(long businessId, string? search, int page, int pageSize);
        Task<Clinic?> GetByIdAsync(long clinicId, long businessId);
        Task<PagedSelect<ClinicSelectItem?>> GetForSelectAsync(long businessId, int page, int pageSize, string? search);
        Task<BaseResponse> UpdateAsync(Clinic entity, IDbTransaction transaction);
        Task<BaseResponse> DeleteAsync(long clinicId, long userId, IDbTransaction transaction);
    }
}
