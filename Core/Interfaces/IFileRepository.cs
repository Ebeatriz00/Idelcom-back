using Core.Entities;
using SharedKernel;
using System;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IFileRepository
    {
        Task<BaseResponse> RegisterFileAsync(SysFile file);
        Task<SysFile?> GetFileByIdAsync(Guid id);
        Task<BaseResponse> DeleteFileAsync(Guid id);
    }
}
