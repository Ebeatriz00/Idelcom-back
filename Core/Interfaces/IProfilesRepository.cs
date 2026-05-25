using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IProfilesRepository
    {

        Task<bool> ExistsAsync(string name, long businessId, long? excludeId = null);
        Task AddAsync(Profiles entity);
        Task<PagedResult<Profiles>> GetAllAsync(int businessId, string? search,int page, int pageSize);
        Task<PagedSelect<OptionItem>> GetProfilesForSelectAsync(long businessId, string? search, int page, int pageSize);

        Task<Profiles> GetByIdAsync(long ProfilesId);
        Task<bool> UpdateAsync(Profiles profiles);
        Task<bool> PatchStatusAsync(long ProfilesId, string status, int UsersBy, long businessId);

    }
}
