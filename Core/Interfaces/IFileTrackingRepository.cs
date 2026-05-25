using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IFileTrackingRepository
    {
        Task<bool> ExistsOpporAsync(string fileUrl, string opporToken,  long businessId, long? excludeId = null);
        Task<bool> ExistsProjectAsync(string fileUrl, string projectToken, long businessId, long? excludeId = null);
        Task AddFileOpporAsync(FileTracking entity);
        Task AddFileProjectAsync(FileTracking entity);
        Task<bool> DeleteFileOpporAsync(string linkToken, string opporToken);
        Task<bool> DeleteFileProjectAsync(string linkToken, string projectToken);
    }
}
