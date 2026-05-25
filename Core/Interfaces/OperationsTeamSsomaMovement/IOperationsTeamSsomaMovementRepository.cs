
using Core.Entities.OperationsTeamSsomaMovement;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.OperationsSsomaMovement
{
    public interface IOperationsTeamSsomaMovementRepository
    {
        Task<BaseResponseId> AddAsync(OperationsTeamSsomaMovement entity, IDbTransaction transaction);
    }
}
