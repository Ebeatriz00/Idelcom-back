using Core.Entities.Operations;
using Core.Entities.paginations;
using Core.Interfaces.Operations;
using Core.Projections.Operations;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using SharedKernel;
using System.Data;

namespace Infrastructure.Repositories.Operations
{
    public class OperationsRepository(IDapperHelper dapperHelper) : IOperationsRepository
    {
        private readonly IDapperHelper _dapperHelper = dapperHelper;

        public async Task<OperationsListItemProjection?> GetByIdAsync(long operationsId)
        {
            var parameters = DapperParams.From(new
            {
                OperationsId = operationsId
            });

            return await _dapperHelper.QueryFirstOrDefaultAsync<OperationsListItemProjection>(
                "SP_WS_GETBYID_OPERATIONS",
                parameters);
        }

        public async Task<PagedResult<OperationsListItemProjection>> GetAllAsync(long businessId, int page, int pageSize)
        {
            var parameters = DapperParams.From(new
            {
                PageSize = pageSize,
                BusinessId = businessId,
                PageNumber = page
            });

            var (items, total) = await _dapperHelper.QueryPagedAsync<OperationsListItemProjection>(
                "SP_WS_GETALL_OPERATIONS",
                parameters);

            return new PagedResult<OperationsListItemProjection>
            {
                Items = items.ToList(),
                Page = page,
                PageSize = pageSize,
                Total = total
            };
        }

        public async Task<BaseResponseId> CreateAsync(Operation entity, long userId, long businessId, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    BusinessId = businessId,
                    entity.OpporId,
                    entity.QualitySupervisorId,
                    entity.ProjectManagerId,
                    entity.RequeredSsoma,
                    entity.PlannedStartDate,
                    entity.ActualStartDate,
                    entity.PlannedEndDate,
                    entity.ActualEndDate,
                    entity.OperationsStatusId,
                    CreateUser = userId
                })
                .WithOutputLong("@OperationsId")
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync(
                    "SP_WS_INSERT_OPERATIONS",
                    parameters,
                    transaction,
                    commandType: CommandType.StoredProcedure);

                var cOutput = parameters.Get<int>("@COutput");
                var sOutput = parameters.Get<string>("@SOutput");
                var id = parameters.Get<long>("@OperationsId");

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

        public async Task<BaseResponse> UpdateAsync(Operation entity, long userId, long businessId, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    entity.OperationsId,
                    BusinessId = businessId,
                    entity.OpporId,
                    entity.QualitySupervisorId,
                    entity.ProjectManagerId,
                    entity.RequeredSsoma,
                    entity.PlannedStartDate,
                    entity.ActualStartDate,
                    entity.PlannedEndDate,
                    entity.ActualEndDate,
                    entity.OperationsStatusId,
                    UpdateUser = userId
                })
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync(
                    "SP_WS_UPDATE_OPERATIONS",
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

        public async Task<BaseResponse> DeleteAsync(long operationsId, long userId, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    OperationsId = operationsId,
                    UpdateUser = userId
                })
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync(
                    "SP_WS_DELETE_OPERATIONS",
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

        public async Task<DateTime?> GetOperationEndDateAsync(long operationsId, IDbTransaction? transaction = null)
        {
            try
            {
                const string sql = @"
                    SELECT TOP 1
                        COALESCE(O.ACTUAL_END_DATE, O.PLANNED_END_DATE)
                    FROM dbo.OPERATIONS O
                    WHERE O.OPERATIONS_ID = @OperationsId
                      AND O.STATUS = '1';";

                return await _dapperHelper.QueryFirstOrDefaultAsync<DateTime?>(
                    sql,
                    DapperParams.From(new
                    {
                        OperationsId = operationsId
                    }),
                    transaction,
                    commandType: CommandType.Text);
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener la fecha fin de la operación.", ex.Message);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al obtener la fecha fin de la operación.", ex.Message);
            }
        }
    }
}
