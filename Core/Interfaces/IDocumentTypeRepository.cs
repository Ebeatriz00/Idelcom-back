using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IDocumentTypeRepository
    {
        Task<bool> ExistsAsync(string description, string codeSunat, int businessId, long? excludeId = null);
        Task AddAsync(DocumentType entity);
        Task<PagedResult<DocumentType>> GetAllAsync(int businessId, string? search, int page, int pageSize, long? usersBy);
        Task<PagedSelect<OptionItem>> GetAreaForSelectAsync(long businessId, string? search, int page, int pageSize);
        Task<DocumentType> GetByIdAsync(long documentTypeId);
        Task<bool> UpdateAsync(DocumentType document);
        Task<bool> PatchStatusAsync(long documentTypeId, string status, int UsersBy, long businessId);
    }
}
