using Core.Entities.Operations;
using Core.Entities.paginations;
using Core.Interfaces.Operations;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;
using SharedKernel;

namespace Infrastructure.Repositories.Operations
{
    public class OperationsWorkOrderProgressRepository(IDapperHelper dapperHelper) : IOperationsWorkOrderProgressRepository
    {
        private readonly IDapperHelper _dapperHelper = dapperHelper;

        public async Task<BaseResponseId> CreateAsync(OperationWorkOrderProgress entity, long userId, long businessId)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    entity.ActivityId,
                    BusinessId = businessId,
                    entity.ReportedQuantity,
                    entity.ReportedDate,
                    WorkerId = userId,
                    entity.Observations,
                    CreateUser = userId
                })
                    .WithOutputLong("@ProgressId")
                    .WithOutputInt("@COutput")
                    .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync("SP_WS_INSERT_WORK_ORDER_PROGRESS", parameters);

                var cOutput = parameters.Get<int>("@COutput");
                var sOutput = parameters.Get<string>("@SOutput");
                var progressId = parameters.Get<long>("@ProgressId");

                if (cOutput != 1)
                    throw new BusinessException(sOutput);

                return new BaseResponseId
                {
                    Status = cOutput,
                    Message = sOutput,
                    Id = progressId
                };
            }
            catch (BaseException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado en el registro de avance de la orden de trabajo.", ex.Message);
            }
        }

        public async Task<(BaseResponseId Response, bool IsDuplicate)> CreateV2Async(OperationWorkOrderProgress entity, long userId, long businessId, string? appRecordId)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    entity.ActivityId,
                    BusinessId = businessId,
                    entity.ReportedQuantity,
                    entity.ReportedDate,
                    WorkerId = entity.WorkerId ?? userId,
                    entity.Observations,
                    CreateUser = userId,
                    AppRecordId = appRecordId
                })
                    .WithOutputBool("@IsDuplicate")
                    .WithOutputLong("@ProgressId")
                    .WithOutputInt("@COutput")
                    .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync("SP_WS_INSERT_WORK_ORDER_PROGRESS", parameters);

                var cOutput = parameters.Get<int>("@COutput");
                var sOutput = parameters.Get<string>("@SOutput");
                var progressId = parameters.Get<long>("@ProgressId");
                var isDuplicate = parameters.Get<bool>("@IsDuplicate");

                if (cOutput != 1)
                    throw new BusinessException(sOutput);

                return (new BaseResponseId
                {
                    Status = cOutput,
                    Message = sOutput,
                    Id = progressId
                }, isDuplicate);
            }
            catch (BaseException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado en la sincronización de avance de la orden de trabajo.", ex.Message);
            }
        }

        public async Task<PagedResult<OperationWorkOrderProgress>> GetAllAsync(
            long businessId,
            string? search,
            int page,
            int pageSize,
            long? activityId,
            string? date,
            long? operationsId)
        {
            var parameters = DapperParams.From(new
            {
                BusinessId = businessId,
                OperationsId = operationsId,
                ActivityId = activityId,
                Search = search,
                ReportedDate = date,
                PageNumber = page,
                PageSize = pageSize
            });

            var result = await _dapperHelper.QueryPagedAsync<OperationWorkOrderProgress>(
                "SP_WS_LIST_WORK_ORDER_PROGRESS",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure);

            return new PagedResult<OperationWorkOrderProgress>
            {
                Items = result.Items.ToList(),
                Page = page,
                PageSize = pageSize,
                Total = result.Total
            };
        }
    }
}
