using Core.Entities.OperationsTeamSsomaMovement;
using Core.Interfaces.OperationsSsomaMovement;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.OperationsSsomaMovement
{
    public class OperationsTeamSsomaMovementRepository(IDapperHelper dapperHelper) : IOperationsTeamSsomaMovementRepository
    {
        private readonly IDapperHelper _dapperHelper = dapperHelper;

        public async Task<BaseResponseId> AddAsync(OperationsTeamSsomaMovement entity, System.Data.IDbTransaction transaction)
        {
            var parameters = DapperParams.From(new
            {
                entity.BusinessId,
                entity.OperationsTeamSsomaId,
                entity.FromSsomaProcessId,
                entity.ToSsomaProcessId,
                entity.Description,
                entity.CreateUser
            })
            .WithOutputLong("@Id")
            .WithOutputInt("@COutput")
            .WithOutputString("@SOutput", 500);

            await _dapperHelper.ExecuteAsync("SP_WS_INSERT_OPERATIONS_SSOMA_MOVEMENT", parameters, transaction);

            var cOutput = parameters.Get<int>("@COutput");
            var sOutput = parameters.Get<string>("@SOutput");
            var id = parameters.Get<long>("@Id");

            if (cOutput != 1)
                throw new BusinessException(sOutput);

            return new BaseResponseId
            {
                Status = cOutput,
                Message = sOutput,
                Id = id
            };
        }
    }
}
