using Core.Entities.OperationsSquad;
using Core.Entities.paginations;
using Core.Interfaces.OperationsSquad;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;
using SharedKernel;
using System.Data;

namespace Infrastructure.Repositories.OperationsSquad
{
    public class OperationsSquadRepository(IDapperHelper dapperHelper) : IOperationsSquadRepository
    {
        private readonly IDapperHelper _dapperHelper = dapperHelper;

        public async Task<BaseResponseId> CreateAsync(OperationSquad entity, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    entity.BusinessId,
                    entity.WorkOrderId,
                    entity.SquadName,
                    entity.TechLeaderId,
                    entity.Description,
                    entity.OperationsProjectConfigId,
                    entity.SquadCategory,
                    entity.CreateUser
                })
                .WithOutputLong("@Id")
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync(
                    "SP_WS_INSERT_OPERATIONS_SQUAD",
                    parameters,
                    transaction);

                var cOutput = parameters.Get<int>("@COutput");
                var sOutput = parameters.Get<string>("@SOutput");
                var id = parameters.Get<long>("@Id");

                return new BaseResponseId
                {
                    Status = cOutput,
                    Message = sOutput,
                    Id = id
                };
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado.", ex.Message);
            }
        }

        public async Task<PagedResult<OperationSquad>> GetAllAsync(long businessId, string? search, int page, int pageSize, long? workOrderId)
        {
            var parameters = DapperParams.From(new
            {
                BusinessId = businessId,
                WorkOrderId = workOrderId,
                Search = search,
                PageNumber = page,
                PageSize = pageSize
            });


            var result = await _dapperHelper.QueryPagedAsync<OperationSquad>(
                "SP_WS_LIST_OPERATIONS_SQUAD",
                parameters,
                commandType: CommandType.StoredProcedure);

            return new PagedResult<OperationSquad>
            {
                Items = result.Items.ToList(),
                Page = page,
                PageSize = pageSize,
                Total = result.Total
            };
        }

        public async Task<OperationSquad?> GetByIdAsync(long squadId)
        {
            var parameters = DapperParams.From(new
            {
                SquadId = squadId
            });

            var result = await _dapperHelper.QueryFirstOrDefaultAsync<OperationSquad>(
                "SP_WS_OPERATIONS_SQUAD_BY_ID",
                parameters);

            return result;
        }


        public async Task<BaseResponse> UpdateAsync(OperationSquad entity, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    entity.SquadId,
                    entity.BusinessId,
                    entity.WorkOrderId,
                    entity.SquadName,
                    entity.TechLeaderId,
                    entity.Description,
                    entity.OperationsProjectConfigId,
                    entity.SquadCategory,
                    entity.UpdateUser
                })
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync(
                    "SP_WS_UPDATE_OPERATIONS_SQUAD",
                    parameters,
                    transaction);

                var cOutput = parameters.Get<int>("@COutput");
                var sOutput = parameters.Get<string>("@SOutput");

                if (cOutput != 1)
                    throw new BusinessException(sOutput);

                return new BaseResponse
                {
                    Status = cOutput,
                    Message = sOutput
                };
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado.", ex.Message);
            }
        }

        public async Task<BaseResponse> DeleteAsync(long squadId, long userId, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    SquadId = squadId,
                    UpdateUser = userId
                })
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync(
                    "SP_WS_DELETE_OPERATIONS_SQUAD",
                    parameters,
                    transaction);

                var cOutput = parameters.Get<int>("@COutput");
                var sOutput = parameters.Get<string>("@SOutput");

                if (cOutput != 1)
                    throw new BusinessException(sOutput);

                return new BaseResponse
                {
                    Status = cOutput,
                    Message = sOutput
                };
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado.", ex.Message);
            }
        }
    }
}
