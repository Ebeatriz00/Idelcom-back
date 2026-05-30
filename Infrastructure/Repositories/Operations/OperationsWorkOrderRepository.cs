using Core.Entities.Operations;
using Core.Entities.paginations;
using Core.Interfaces.Operations;
using Core.Projections.Operations;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;
using Dapper;
using SharedKernel;
using System.Data;

namespace Infrastructure.Repositories.Operations
{
    public class OperationsWorkOrderRepository(IDapperHelper dapperHelper) : IOperationsWorkOrderRepository
    {

        private readonly IDapperHelper _dapperHelper = dapperHelper;

        public async Task<BaseResponseId> CreateAsync(OperationWorkOrder entity, long userId, long businessId, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    BusinessId = businessId,
                    entity.OperationsId,
                    entity.WorkOrderName,
                    entity.OrderStatusId,
                    entity.StartDate,
                    entity.EndDate,
                    entity.Location,
                    entity.NeedLogistics,
                    entity.NeedSsoma,
                    entity.NeedAttendance,
                    entity.IsAdministrative,
                    entity.TechLeaderId,
                    entity.Description,
                    entity.OperationsProjectConfigId,
                    CreateUser = userId
                })
                .WithOutputLong("@Id")
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync("SP_WS_INSERT_OPERATIONS_WORK_ORDER", parameters, transaction);

                var cOutput = parameters.Get<int>("@COutput");
                var sOutput = parameters.Get<string>("@SOutput");

                if (cOutput != 1)
                    throw new BusinessException(sOutput);

                var id = parameters.Get<long>("@Id");

                return new BaseResponseId
                {
                    Status = cOutput,
                    Message = sOutput,
                    Id = id
                };
            }
            catch (BaseException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado en la creación de la Work Order.", ex.Message);
            }
        }

        public async Task<BaseResponse> DeleteAsync(long workOrderId, long businessId, long userId, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    WorkOrderId = workOrderId,
                    BusinessId = businessId,
                    UpdateUser = userId
                })
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync("SP_WS_DELETE_OPERATIONS_WORK_ORDER", parameters, transaction);

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
            catch (BaseException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al eliminar la Work Order.", ex.Message);
            }
        }

        public async Task<PagedResult<OperationWorkOrder>> GetAllAsync(long businessId, int page, int pageSize, string? search, long? operationsId)
        {
            var parameters = DapperParams.From(new
            {
                BusinessId = businessId,
                OperationsId = operationsId,
                PageNumber = page,
                PageSize = pageSize,
                Search = search
            });

            var (item, total) = await _dapperHelper.QueryPagedAsync<OperationWorkOrder>("SP_WS_LIST_OPERATIONS_WORK_ORDER", parameters);

            return new PagedResult<OperationWorkOrder>
            {
                Items = item.ToList(),
                Page = page,
                PageSize = pageSize,
                Total = total
            };
        }

        public async Task<OperationWorkOrder?> GetByIdAsync(long workOrderId, long businessId)
        {
            var parameters = DapperParams.From(new
            {
                BusinessId = businessId,
                WorkOrderId = workOrderId
            });

            return await _dapperHelper.QueryFirstOrDefaultAsync<OperationWorkOrder>("SP_WS_GETBYID_OPERATIONS_WORK_ORDER", parameters);
        }

        public async Task<OperationWorkOrder?> GetByIdAsync(long workOrderId, long businessId, IDbTransaction transaction)
        {
            var parameters = DapperParams.From(new
            {
                BusinessId = businessId,
                WorkOrderId = workOrderId
            });

            return await _dapperHelper.QueryFirstOrDefaultAsync<OperationWorkOrder>(
                "SP_WS_GETBYID_OPERATIONS_WORK_ORDER",
                parameters,
                transaction);
        }

        public async Task<BaseResponse> UpdateAsync(OperationWorkOrder entity, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    entity.WorkOrderId,
                    entity.BusinessId,
                    entity.WorkOrderName,
                    entity.OrderStatusId,
                    entity.StartDate,
                    entity.EndDate,
                    entity.Location,
                    entity.NeedLogistics,
                    entity.NeedSsoma,
                    entity.NeedAttendance,
                    entity.IsAdministrative,
                    entity.UpdateUser
                })
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync("SP_WS_UPDATE_OPERATIONS_WORK_ORDER", parameters, transaction);

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
            catch (BaseException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al actualizar la Work Order.", ex.Message);
            }
        }

        public async Task<(IEnumerable<OperationsWorkOrderSummaryProjection> Summaries, IEnumerable<OperationsWorkOrderProgressDetailProjection> Details)> GetProgressReportAsync(long businessId, long operationsId)
        {
            var parameters = DapperParams.From(new
            {
                BUSINESS_ID = businessId,
                OPERATIONS_ID = operationsId
            });

            return await _dapperHelper.QueryMultipleAsync(
                "SP_WS_GET_OPERATIONS_PROGRESS_REPORT",
                async (multi) =>
                {
                    var summaries = await multi.ReadAsync<OperationsWorkOrderSummaryProjection>();
                    var details = await multi.ReadAsync<OperationsWorkOrderProgressDetailProjection>();
                    return (summaries, details);
                },
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

    }
}
