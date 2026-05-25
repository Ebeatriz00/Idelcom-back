using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IUsersPreferencesRepository
    {
        Task<UsersPreferences> GetNotifByIdAsync(long usersId, long businessId);
        Task<UsersPreferences> GetPreferencesByIdAsync(long usersId, long businessId);
        Task<UsersPreferences> GetSettingByIdAsync(long usersId, long businessId);

        Task<bool> UpdateNotifAsync(UsersPreferences users);
        Task<bool> UpdatePreferencesAsync(UsersPreferences users);
        Task<bool> UpdateSettingAsync(UsersPreferences users);

    }
}
