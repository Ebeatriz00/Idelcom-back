using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IUsersRepository
    {
        Task<bool> ExistsAsync(string userDocument, long businessId, long? excludeId = null);
        Task<bool> GetLastUserCodeAsync(string usersCode,long businessId, long? excludeId = null);
        Task AddAsync(Users entity);
        Task<PagedResult<Users>> GetAllAsync(int businessId, string? search, int page, int pageSize);
        Task<bool> ExistsCodeAsync(string usersCode, long businessId);
        Task<Users> GetByIdAsync(long usersId);
        Task<bool> UpdateAsync(Users users);
        Task<bool> PasswordChangeAsync(Users entity);
        Task<bool> PatchStatusAsync(long usersId, string status, int UsersBy, long businessId);

        Task<Users> GetSettingByIdAsync(long usersId);
        Task<bool> UpdateSettingAsync(Users users);

    }
}
