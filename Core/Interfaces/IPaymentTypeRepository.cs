using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IPaymentTypeRepository
    {
        Task<bool> ExistsAsync(string code, string description, long businessId, long? excludeId = null);
        Task AddAsync(PaymentType entity);
        Task<PagedResult<PaymentType>> GetAllAsync(long businessId, string? search, int page, int pageSize, long? usersBy);
        Task<PagedSelect<OptionItem>> GetPaymentTypeForSelectAsync(long businessId, string? search, int page, int pageSize);
        Task<PaymentType> GetByIdAsync(long paymentTypeId);
        Task<bool> UpdateAsync(PaymentType paymentType);
        Task<bool> PatchStatusAsync(long paymentTypeId, string status, int UsersBy, long businessId);
    }
}
