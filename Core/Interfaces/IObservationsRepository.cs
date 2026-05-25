using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IObservationsRepository
    {
        Task AddAsync(IEnumerable<Observations> entities);
        Task<Observations> GetByIdAsync(long obsId);
        Task<IEnumerable<Observations>> GetAllByOpporIdAsync(long opporId);
        Task<IEnumerable<Observations>> GetAllByProjectAsync(long opporId);
        Task<IEnumerable<Observations>> GetAllByHiringAsync(long opporId);
        Task<bool> UpdateReasonAsync(Observations entity);
        Task<bool> UpdateDueDateAsync(Observations entity);
    }
}
