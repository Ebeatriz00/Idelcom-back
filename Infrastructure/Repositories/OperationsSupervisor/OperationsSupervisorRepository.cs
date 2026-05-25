using Core.Entities.OperationsSupervisor;
using Core.Entities.paginations;
using Core.Interfaces.OperationsSupervisor;
using Dapper;
using Infrastructure.Exceptions;
using SharedKernel;
using System.Data;

namespace Infrastructure.Persistence.Repositories
{
    public class OperationsSupervisorRepository : IOperationsSupervisorRepository
    {
        private readonly IDapperHelper _dapperHelper;

        public OperationsSupervisorRepository(IDapperHelper dapperHelper)
        {
            _dapperHelper = dapperHelper;
        }

        public async Task<BaseResponseId> CreateAsync(OperationSupervisor entity, long businessId, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    entity.OperationsId,
                    entity.WorkerId,
                    entity.IsMain,
                    entity.CreateUser
                })
                .WithOutputLong("@Id")
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync(
                    "SP_WS_INSERT_OPERATIONS_SUPERVISOR",
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

        public async Task<PagedResult<OperationSupervisor>> GetAllAsync(long businessId, string? search, int page, int pageSize)
        {
            var parameters = DapperParams.From(new
            {
                BusinessId = businessId,
                Search = search,
                PageNumber = page,
                PageSize = pageSize
            });

            var result = await _dapperHelper.QueryPagedAsync<OperationSupervisor>(
                "SP_WS_LIST_OPERATIONS_SUPERVISOR",
                parameters);

            return new PagedResult<OperationSupervisor>
            {
                Items = result.Items.ToList(),
                Page = page,
                PageSize = pageSize,
                Total = result.Total
            };
        }

        public async Task<BaseResponse> UpdateAsync(OperationSupervisor entity, long businessId, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    entity.SupervisorId,
                    entity.OperationsId,
                    entity.WorkerId,
                    entity.IsMain,
                    entity.UpdateUser
                })
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync(
                    "SP_WS_UPDATE_OPERATIONS_SUPERVISOR",
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

        public async Task<BaseResponse> DeleteAsync(long supervisorId, long userId, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    SupervisorId = supervisorId,
                    UpdateUser = userId
                })
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync(
                    "SP_WS_DELETE_OPERATIONS_SUPERVISOR",
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

        public async Task<OperationSupervisor> GetByIdAsync(long supervisorId)
        {
            var parameters = new DapperParams();
            parameters.Add("@SupervisorId", supervisorId);

            var result = await _dapperHelper.QueryFirstOrDefaultAsync<OperationSupervisor>(
                "SP_WS_OPERATIONS_SUPERVISOR_BY_ID",
                parameters,
                commandType: CommandType.StoredProcedure);

            return result;
        }
    }
}
