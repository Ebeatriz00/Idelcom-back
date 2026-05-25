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
    public interface IHiringRepository
    {
        Task<GlobalResponse> AddAsync(Hiring entity);
        Task<PagedResult<Hiring>> GetAllAsync(long businessId, string? search, int page, int pageSize, long? workerId, long? usersId);
        Task<GlobalResponse> UpdateStatusAsync(Hiring entity);
        Task MarkFilesReadAsync(FileTracking entity);
    }
}
