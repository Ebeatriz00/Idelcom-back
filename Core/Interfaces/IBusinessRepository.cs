using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IBusinessRepository
    {
        Task<bool> ExistsAsync(string ruc_business, string code_license, long businessId, long? excludeId = null);
        //Task<string> GetLastBusinessCodeAsync(long businessId);
        Task AddAsync(Business entity);
        Task<Business> GetByViewAsync(long businessId);
        //Task<List<Business>> GetAllAsync(int businessId);
        //Task<Business> GetByIdAsync(long businessId);
        //Task<bool> UpdateAsync(Business currency);
        //Task<bool> PatchStatusAsync(long businessId, string status, int UsersBy);


    }
}
