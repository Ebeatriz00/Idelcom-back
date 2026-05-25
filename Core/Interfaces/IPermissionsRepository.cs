using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{

        public interface IPermissionsRepository
        {
            Task<bool> ExistsAsync(string permissionName, long businessId, long? excludeId = null);
            Task AddAsync(Permissions entity);
            Task<PagedResult<Permissions>> GetAllAsync(long businessId, int page, int pageSize);
            Task<PagedSelect<OptionItem>> GetPermissionsForSelectAsync(long businessId, string? search, int page, int pageSize);
            Task<Permissions> GetByIdAsync(long permissionId);
            Task<bool> UpdateAsync(Permissions permission);
            Task<bool> PatchStatusAsync(long permissionId, string status, int updatedBy, long businessId);
        }
}

