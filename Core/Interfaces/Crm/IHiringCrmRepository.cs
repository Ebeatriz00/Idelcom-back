using Core.Entities.Crm;
using SharedKernel;
using System.Data;

namespace Core.Interfaces.Crm
{
    public interface IHiringCrmRepository
    {
        Task<GlobalResponse> AddAsync(HiringCrm entity, IDbTransaction transaction);
    }
}
