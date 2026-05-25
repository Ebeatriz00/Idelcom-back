using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ICommercialParametersRepository
    {
        Task<bool> ExistsAsync(string description, long businessId, long? excludeId = null);
        Task AddAsync(CommercialParameters entity);
        Task<PagedResult<CommercialParameters>> GetAllAsync(long businessId, string? search, int page, int pageSize, long? usersBy);
       Task<CommercialParameters> GetByIdAsync(int commercialParametersId);
        Task<bool> UpdateAsync(CommercialParameters entity);
        Task<bool> PatchStatusAsync(int commercialParametersId, string status, long updatedBy, long businessId);
    }
}
