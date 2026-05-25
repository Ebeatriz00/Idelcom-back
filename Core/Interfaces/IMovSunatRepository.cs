using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IMovSunatRepository
    {
        Task<bool> ExistsAsync(string description, long businessId, long? excludeId = null);
        Task AddAsync(MovSunat entity);
        Task<PagedResult<MovSunat>> GetAllAsync(long businessId, string? search, int page, int pageSize);
        Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, string? search, int page, int pageSize);
        Task<MovSunat> GetByIdAsync(long movSunatId);
        Task<bool> UpdateAsync(MovSunat entity);
        Task<bool> PatchStatusAsync(long movSunatId, string status, long usersBy, long businessId);
    }
}
