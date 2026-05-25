using Core.Entities;
using Core.Entities.paginations;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IOpportunitiesRepository
    {
        Task<bool> ExistsAsync(string oppDesc, long businessId, string? excludeId = null);
        Task<string> GetCodeAsync(long businessId);
        Task<GlobalResponse> AddAsync(Opportunities entity);
        Task<GlobalResponse> AttachHiringFilesAsync(long businessId, long opporId, long usersBy, List<OpportFiletracking> files);
        Task<PagedResult<Opportunities>> GetAllAsync(long businessId, string? search, int page, int pageSize, long? usersId, long? stateId, long? workerId, DateTime? filterStartDate, DateTime? filterFinishDate, int? filterYear);
        Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, long clientsId, string? search, int page, int pageSize);

        Task<Opportunities> GetByIdAsync(string linkToken);
        Task<GlobalResponse> UpdateAsync(Opportunities opportunities);
        Task<bool> PatchStatusAsync(string linkToken, string status, long UsersBy, long businessId);
        Task<Opportunities> GetDetailAsync(string linkToken, long businessId, long? usersId, CancellationToken ct = default);

        Task<Opportunities> GetClientsByIdAsync(string linkToken);
        Task<Opportunities> GetStateByIdAsync(string linkToken);

        Task<bool> UpdateClientsAsync(Opportunities opportunities);
        Task<bool> UpdateStateAsync(Opportunities opportunities);
        Task<GlobalResponse> UpdateDeliverablesOnlyAsync(Opportunities opportunities);


        //*=============================DELIVERABLES=================================*//
        Task<PagedSelect<OptionItem>> GetForSelectDeliverablesAsync(long businessId, string? search, int page, int pageSize);
        Task<PagedSelect<OptionItem>> GetForSelectDeliverablesHiringAsync(long businessId, string? search, int page, int pageSize);
        //*=============================VERSINAMIENTO DE COTIZACION=================================*//
        Task<bool> UploadNewVerAsync(Opportunities entity);
        Task<PagedSelect<OptionItem>> GetForQuoVerSelectAsync(long businessId, string linkToken, string? search, int page, int pageSize);

        //*============================= TIPO DE ATENCION=================================*//
        Task<PagedSelect<OptionItem>> GetForFlowTypeSelectAsync(long businessId, string? search, int page, int pageSize);

    }
}
