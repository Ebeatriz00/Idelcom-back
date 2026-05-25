using Core.Entities.Operations;
using Core.Entities.paginations;
using Core.Interfaces.Operations;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;
using SharedKernel;
using System.Data;

namespace Infrastructure.Repositories.Operations
{
    public class SupportRepository(IDapperHelper dapperHelper) : ISupportRepository
    {
        private readonly IDapperHelper _dapperHelper = dapperHelper;

        public async Task<BaseResponseId> CreateAsync(Support entity, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    entity.BusinessId,
                    entity.Provider,
                    entity.Service,
                    entity.Url,
                    entity.Access,
                    entity.Email,
                    entity.Username,
                    entity.Password,
                    entity.SupportState,
                    entity.StartDate,
                    entity.ExpirationDate,
                    entity.Comments,
                    entity.Remarks,
                    entity.CreateUser
                })
                .WithOutputLong("@Id")
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync(
                    "SP_WS_INSERT_SUPPORT",
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
                throw new DatabaseException(ex.Message, ex.Message);
            }
        }

        public async Task<PagedResult<Support>> GetAllAsync(long businessId, string? search, int page, int pageSize)
        {
            var parameters = DapperParams.From(new
            {
                BusinessId = businessId,
                Search = search,
                PageNumber = page,
                PageSize = pageSize
            });

            var result = await _dapperHelper.QueryPagedAsync<Support>(
                "SP_WS_LIST_SUPPORT",
                parameters,
                commandType: CommandType.StoredProcedure);

            return new PagedResult<Support>
            {
                Items = result.Items.ToList(),
                Page = page,
                PageSize = pageSize,
                Total = result.Total
            };
        }

        public async Task<Support?> GetByIdAsync(long supportId, long businessId)
        {
            var parameters = DapperParams.From(new
            {
                SupportId = supportId,
                BusinessId = businessId
            });

            var result = await _dapperHelper.QueryFirstOrDefaultAsync<Support>(
                "SP_WS_SUPPORT_BY_ID",
                parameters);

            return result;
        }

        public async Task<BaseResponse> UpdateAsync(Support entity, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    entity.SupportId,
                    entity.BusinessId,
                    entity.Provider,
                    entity.Service,
                    entity.Url,
                    entity.Access,
                    entity.Email,
                    entity.Username,
                    entity.Password,
                    entity.SupportState,
                    entity.StartDate,
                    entity.ExpirationDate,
                    entity.Comments,
                    entity.Remarks,
                    entity.UpdateUser
                })
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync(
                    "SP_WS_UPDATE_SUPPORT",
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
                throw new DatabaseException(ex.Message, ex.Message);
            }
        }

        public async Task<BaseResponse> DeleteAsync(long supportId, long userId, long businessId, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    SupportId = supportId,
                    BusinessId = businessId,
                    UpdateUser = userId
                })
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync(
                    "SP_WS_DELETE_SUPPORT",
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
                throw new DatabaseException(ex.Message, ex.Message);
            }
        }
    }
}
