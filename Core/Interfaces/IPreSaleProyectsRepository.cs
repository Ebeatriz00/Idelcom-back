using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IPreSaleProyectsRepository
    {
        Task<bool> ExistsAsync(string description, long businessId, string? exclude = null);
        Task<string> GetCodeAsync(long businessId);
        Task AddAsync(PreSaleProyects entity);
        Task<PagedResult<PreSaleProyects>> GetAllAsync(
        long businessId,
        string? search,
        int page,
        int pageSize,
        long? workerId,
        string? filterCode = null,
        string? filterProject = null,
        string? filterClient = null,
        string? filterSeller = null,
        string? filterResponsible = null,
        string? filterStatePreSale = null,
        string? filterStateOpportunity = null,
        string? filterFinishDate = null,
        DateTime? filterDateFrom = null,
        DateTime? filterDateTo = null,
        string? opporNum = null,
        long? usersId = null,
        string? sortBy = null,      
        string? sortDirection = null,
        long? stateId = null,
        int? caegory = null,
        string? quoDate = null
);
        Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, string? search, int page, int pageSize);
        Task<PreSaleProyects> GetByIdAsync(string linkToken);
        Task<bool> UpdateAsync(PreSaleProyects entity);
        Task<bool> PatchStatusAsync(string linkToken, string status, long usersBy, long businessId);
        Task<PreSaleProyects> GetDetailAsync(string linkToken, long businessId, CancellationToken ct = default);
        Task<bool> UpdateResponsibleAsync(PreSaleProyects entity);
        Task<bool> UpdateStateAsync(PreSaleProyects preSaleProyects);



    }
}
