using Core.Interfaces.Operations;
using Infrastructure.Persistence;
using SharedKernel;
using System;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Operations
{
    public class OperationsWorkOrderProgressPhotoRepository(IDapperHelper dapperHelper) : IOperationsWorkOrderProgressPhotoRepository
    {
        private readonly IDapperHelper _dapperHelper = dapperHelper;

        public async Task<BaseResponse> InsertPhotoAsync(long progressId, Guid fileUid, long createdBy)
        {
            var parameters = DapperParams.From(new
            {
                ProgressId = progressId,
                FileUid = fileUid,
                CreatedBy = createdBy
            })
            .WithOutputInt("@COutput")
            .WithOutputString("@SOutput", 500);

            await _dapperHelper.ExecuteAsync("SP_WS_INSERT_OPERATIONS_WORK_ORDER_PROGRESS_PHOTO", parameters);

            return new BaseResponse
            {
                Status = parameters.Get<int>("@COutput"),
                Message = parameters.Get<string>("@SOutput")
            };
        }

        public async Task<IEnumerable<Guid>> GetPhotosByProgressIdAsync(long progressId)
        {
            return await _dapperHelper.QueryAsync<Guid>(
                "SP_WS_GET_OPERATIONS_WORK_ORDER_PROGRESS_BY_PROGRESS_ID",
                new { ProgressId = progressId }
            );
        }
    }
}
