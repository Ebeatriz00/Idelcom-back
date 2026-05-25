using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ISsomaDocumentTypeRepository
    {
        Task AddAsync(SsomaDocumentType entity);
        Task<PagedResult<SsomaDocumentType>> GetAllAsync(long businessId, string? search, int page, int pageSize, long? usersBy);
        Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, string? search, int page, int pageSize);
        Task<SsomaDocumentType> GetByIdAsync(long SsomaDocumentTypeId);
        Task<bool> UpdateAsync(SsomaDocumentType entity);
        Task<bool> PatchStatusAsync(long SsomaDocumentTypeId, string status, long usersBy, long businessId);
    }
}
