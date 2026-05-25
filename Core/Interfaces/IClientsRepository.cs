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
    public interface IClientsRepository
    {
        Task<(bool exists,
          bool sameSeller,
          long? dbWorkerId,
          string? vendedor,
          string? clienteDb,
          string? lastActivityDesc,
          DateTime? lastActivityAt)>
        ExistsAsync(string documents, long businessId, long? workerId, long? excludeId = null);

        Task<bool> ExistsContacts(long clientsId);

        Task<GlobalResponse> AddAsync(Clients entity);
        Task<PagedResult<Clients>> GetAllAsync(long businessId, string? search, int page, int pageSize, long? usersBy, bool? includeOthers);
        Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, string? search, int page, int pageSize,long? usersBy);
        Task<List<Clients>> GetHistoryAsync(long? clientsId);
        Task<Clients> GetByIdAsync(long clientsId);
        Task<bool> UpdateAsync(Clients entity);
        Task<bool> UpdateChangeSalesAsync(Clients entity);
        Task<bool> PatchStatusAsync(long clientsId, string status, long usersBy, long businessId);
        Task<ClientsDetailDto> GetClientsDetailAsync(long clientsId, long businessId);
    }
}
