using Core.Entities.Operations.Core.Entities;
using Core.Entities.paginations;
using Core.Interfaces.Operations;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;
using SharedKernel;
using System.Data;

namespace Infrastructure.Repositories.Operations
{
    public class OperationsWorkOrderResponsibleRepository(IDapperHelper dapperHelper) : IOperationsWorkOrderResponsibleRepository
    {
        private readonly IDapperHelper _dapperHelper = dapperHelper;

        public async Task<BaseResponseId> CreateAsync(OperationWorkOrderResponsible entity, long userId, long businessId, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    BusinessId = businessId,
                    entity.WorkOrderId,
                    entity.WorkerId,
                    entity.IsMain,
                    CreateUser = userId
                })
                    .WithOutputLong("@Id")
                    .WithOutputInt("@COutput")
                    .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync("SP_WS_INSERT_OPERATIONS_WORK_ORDER_RESPONSIBLE", parameters, transaction);

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
            catch (BaseException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado en la creación de la Work Order Responsible.", ex.Message);
            }
        }

        public async Task<BaseResponse> UpdateAsync(OperationWorkOrderResponsible entity, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    entity.WorkOrderResponsibleId,
                    entity.BusinessId,
                    entity.WorkOrderId,
                    entity.WorkerId,
                    entity.IsMain,
                    entity.UpdateUser
                })
                    .WithOutputInt("@COutput")
                    .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync("SP_WS_UPDATE_OPERATIONS_WORK_ORDER_RESPONSIBLE", parameters, transaction);

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
            catch (Exception es)
            {
                throw new DatabaseException("Error inesperado en la eliminación de la Work Order Responsible.", es.Message);
            }
        }

        public async Task<BaseResponse> DeleteAsync(long workOrderResponsibleId, long businessId, long userId, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    WorkOrderResponsibleId = workOrderResponsibleId,
                    BusinessId = businessId,
                    UpdateUser = userId
                })
                    .WithOutputInt("@COutput")
                    .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync("SP_WS_DELETE_OPERATIONS_WORK_ORDER_RESPONSIBLE", parameters, transaction);

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
            catch (Exception es)
            {
                throw new DatabaseException("Error inesperado en la eliminación de la Work Order Responsible.", es.Message);
            }
        }

        public async Task<PagedResult<OperationWorkOrderResponsible>> GetAllAsync(long businessId, int page, int pageSize, string? search)
        {
            var parameters = DapperParams.From(new
            {
                BusinessId = businessId,
                PageNumber = page,
                PageSize = pageSize,
                Search = search
            });

            var (item, total) = await _dapperHelper.QueryPagedAsync<OperationWorkOrderResponsible>("SP_WS_LIST_OPERATIONS_WORK_ORDER_RESPONSIBLE", parameters);

            return new PagedResult<OperationWorkOrderResponsible>
            {
                Items = item.ToList(),
                Page = page,
                PageSize = pageSize,
                Total = total
            };
        }

        public async Task<OperationWorkOrderResponsible?> GetByIdAsync(long workOrderResponsibleId, long businessId)
        {
            var parameters = DapperParams.From(new
            {
                BusinessId = businessId,
                WorkOrderResponsibleId = workOrderResponsibleId
            });

            return await _dapperHelper.QueryFirstOrDefaultAsync<OperationWorkOrderResponsible>("SP_WS_GETBYID_OPERATIONS_WORK_ORDER_RESPONSIBLE", parameters);
        }

        public async Task<OperationWorkOrderResponsible?> GetByIdAsync(long workOrderResponsibleId, long businessId, IDbTransaction transaction)
        {
            var parameters = DapperParams.From(new
            {
                BusinessId = businessId,
                WorkOrderResponsibleId = workOrderResponsibleId,
            });

            return await _dapperHelper.QueryFirstOrDefaultAsync<OperationWorkOrderResponsible>("SP_WS_GETBYID_OPERATIONS_WORK_ORDER_RESPONSIBLE", parameters, transaction);
        }
    }
}
