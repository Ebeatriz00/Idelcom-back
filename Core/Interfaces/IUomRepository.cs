using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
        public interface IUomRepository
        {
            Task<bool> ExistsAsync(string description, string codeSunat, long businessId, long? excludeId = null);
            Task<string> GetLastUomCodeAsync(long businessId);
            Task AddAsync(Uom entity);
            Task<PagedResult<Uom>> GetAllAsync(long businessId, string? search, int page, int pageSize, long? usersBy);
            Task<PagedSelect<OptionItem>> GetUomForSelectAsync(long businessId, string? search, int page, int pageSize);
            Task<Uom> GetByIdAsync(long uomId);
            Task<bool> UpdateAsync(Uom uom);
            Task<bool> PatchStatusAsync(long uomId, string status, int usersBy, long businessId);
        }
    }


