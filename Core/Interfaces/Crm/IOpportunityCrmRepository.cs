using Core.Entities;
using Core.Entities.Crm;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Crm
{
    public interface IOpportunityCrmRepository
    {
        Task<bool> ExistsAsync(string opporDesc, long businessId, long? excludeId = null);
        Task<BaseResponseId> AddAsync(OpportunityCrm entity, IDbTransaction transaction);
    }
}
