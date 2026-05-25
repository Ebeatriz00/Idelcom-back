using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IMedicalAptitudeRepository
    {
        Task AddAsync(MedicalAptitude entity);
        Task<PagedResult<MedicalAptitude>> GetAllAsync(long businessId, string? search, int page, int pageSize, long? usersBy);
        Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, string? search, int page, int pageSize);
        Task<MedicalAptitude> GetByIdAsync(long medicalAptitudeId);
        Task<bool> UpdateAsync(MedicalAptitude entity);
        Task<bool> PatchStatusAsync(long medicalAptitudeId, string status, long usersBy, long businessId);
    }
}
