using Core.Entities.Operations;
using Core.Entities.paginations;
using Core.Interfaces.Operations;
using Core.Projections.Operations;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;
using SharedKernel;
using System.Data;

namespace Infrastructure.Repositories.Operations
{
    public class OperationsWorkOrderActivityRepository(IDapperHelper dapperHelper) : IOperationsWorkOrderActivityRepository
    {
        private readonly IDapperHelper _dapperHelper = dapperHelper;

        public async Task<BaseResponseId> CreateAsync(OperationWorkOrderActivity entity, long userId, long businessId, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    entity.WorkOrderId,
                    BusinessId = businessId,
                    entity.ActivityName,
                    entity.MeasurementUnitId,
                    entity.ComplexityId,
                    entity.TargetQuantity,
                    CreateUser = userId
                })
                    .WithOutputLong("@ActivityId")
                    .WithOutputInt("@COutput")
                    .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync("SP_WS_INSERT_WORK_ORDER_ACTIVITY", parameters, transaction);

                var cOutput = parameters.Get<int>("@COutput");
                var sOutput = parameters.Get<string>("@SOutput");
                var activityId = parameters.Get<long>("@ActivityId");

                if (cOutput != 1)
                    throw new BusinessException(sOutput);

                return new BaseResponseId
                {
                    Status = cOutput,
                    Message = sOutput,
                    Id = activityId
                };
            }
            catch (BaseException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado en la creación de la actividad de la orden de trabajo.", ex.Message);
            }
        }

        public async Task<OperationWorkOrderActivity?> GetByIdAsync(long activityId, long businessId)
        {
            var parameters = DapperParams.From(new
            {
                ActivityId = activityId,
                BusinessId = businessId
            });

            return await _dapperHelper.QueryFirstOrDefaultAsync<OperationWorkOrderActivity>(
                "SP_WS_GETBYID_WORK_ORDER_ACTIVITY",
                parameters);
        }

        public async Task<OperationWorkOrderActivity?> GetByIdAsync(long activityId, long businessId, IDbTransaction transaction)
        {
            var parameters = DapperParams.From(new
            {
                ActivityId = activityId,
                BusinessId = businessId
            });

            return await _dapperHelper.QueryFirstOrDefaultAsync<OperationWorkOrderActivity>(
                "SP_WS_GETBYID_WORK_ORDER_ACTIVITY",
                parameters, transaction);
        }

        public async Task<PagedResult<OperationWorkOrderActivity>> GetAllAsync(
            long workOrderId,
            long businessId,
            int page,
            int pageSize,
            string? search)
        {
            var parameters = DapperParams.From(new
            {
                WorkOrderId = workOrderId,
                BusinessId = businessId,
                PageNumber = page,
                PageSize = pageSize,
                Search = search
            });

            var (items, total) = await _dapperHelper.QueryPagedAsync<OperationWorkOrderActivity>(
                "SP_WS_LIST_WORK_ORDER_ACTIVITY",
                parameters);

            return new PagedResult<OperationWorkOrderActivity>
            {
                Items = items.ToList(),
                Page = page,
                PageSize = pageSize,
                Total = total
            };
        }
           
        public async Task<IEnumerable<AppActivityWorkOrderProjection>> GetAppActivitiesByResponsibleAsync(long userId, long businessId)
        {
            var parameters = DapperParams.From(new
            {
                UserId = userId,
                BusinessId = businessId
            });

            return await _dapperHelper.QueryAsync<AppActivityWorkOrderProjection>("SP_WS_APP_GET_ACTIVITIES_BY_RESPONSIBLE", parameters);
        }

        public async Task<BaseResponse> UpdateAsync(OperationWorkOrderActivity entity, long userId, long businessId, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    entity.ActivityId,
                    entity.WorkOrderId,
                    BusinessId = businessId,
                    entity.ActivityName,
                    entity.MeasurementUnitId,
                    entity.ComplexityId,
                    entity.TargetQuantity,
                    UpdateUser = userId
                })
                    .WithOutputInt("@COutput")
                    .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync("SP_WS_UPDATE_WORK_ORDER_ACTIVITY", parameters, transaction);

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
                throw new DatabaseException("Error inesperado en la actualización de la actividad de la orden de trabajo.", ex.Message);
            }
        }

        public async Task<BaseResponse> DeleteAsync(long activityId, long businessId, long userId, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    ActivityId = activityId,
                    BusinessId = businessId,
                    UpdateUser = userId
                })
                    .WithOutputInt("@COutput")
                    .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync("SP_WS_DELETE_WORK_ORDER_ACTIVITY", parameters, transaction);

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
                throw new DatabaseException("Error inesperado en la eliminación de la actividad de la orden de trabajo.", ex.Message);
            }
        }

        public async Task<PagedSelect<OperationsWorkOrderActivitySelectItem?>> GetForSelectAsync(long businessId, long operationsId, int page, int pageSize, string? search)
        {
            var parameters = DapperParams.From(new
            {
                BusinessId = businessId,
                OperationsId = operationsId,
                PageNumber = page,
                PageSize = pageSize,
                Search = search
            });

            var result = await _dapperHelper.QueryAsync<OperationsWorkOrderActivitySelectItem>("SP_WS_SELECT_WORK_ORDER_ACTIVITY", parameters);
            
            return new PagedSelect<OperationsWorkOrderActivitySelectItem?>
            {
                Items = result.ToList(),
                Page = page,
                PageSize = pageSize,
                HasMore = false
            };
        }
    }
}
