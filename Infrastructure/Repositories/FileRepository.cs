using Core.Entities;
using Core.Interfaces;
using Infrastructure.Persistence;
using SharedKernel;

namespace Infrastructure.Repositories
{
    public class FileRepository(IDapperHelper dapperHelper) : IFileRepository
    {
        private readonly IDapperHelper _dapperHelper = dapperHelper;

        public async Task<BaseResponse> RegisterFileAsync(SysFile file)
        {
            var parameters = DapperParams.From(new
            {
                FileUid = file.Id,
                file.RelativePath,
                file.OriginalName,
                FileSize = file.FileSizeBytes,
                file.MimeType,
                CreatedBy = file.CreatedByUserId
            })
            .WithOutputInt("@COutput")
            .WithOutputString("@SOutput", 500);

            await _dapperHelper.ExecuteAsync("SP_SYS_REGISTER_FILE", parameters);

            return new BaseResponse
            {
                Status = parameters.Get<int>("@COutput"),
                Message = parameters.Get<string>("@SOutput")
            };
        }

        public async Task<SysFile?> GetFileByIdAsync(Guid id)
        {
            var result = await _dapperHelper.QueryFirstOrDefaultAsync<dynamic>(
                "SP_SYS_GET_FILE",
                new { FileUid = id });

            if (result == null) return null;

            return new SysFile
            {
                RelativePath = result.RELATIVE_PATH,
                OriginalName = result.ORIGINAL_NAME,
                FileSizeBytes = result.FILE_SIZE_BYTES,
                MimeType = result.MIME_TYPE
            };
        }

        public async Task<BaseResponse> DeleteFileAsync(Guid id)
        {
            var parameters = DapperParams.From(new { FileUid = id })
            .WithOutputInt("@COutput")
            .WithOutputString("@SOutput", 500);

            await _dapperHelper.ExecuteAsync("SP_SYS_DELETE_FILE", parameters);

            return new BaseResponse
            {
                Status = parameters.Get<int>("@COutput"),
                Message = parameters.Get<string>("@SOutput")
            };
        }
    }
}
