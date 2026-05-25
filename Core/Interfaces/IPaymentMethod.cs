using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IPaymentMethodRepository
    {
        Task<bool> ExistsAsync(string description, long businessId, long? excludeId = null);
        Task AddAsync(PaymentMethod entity);
        Task<PagedResult<PaymentMethod>> GetAllAsync(long businessId, string? search, int page, int pageSize);
        Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, string? search, int page, int pageSize);
        Task<PaymentMethod> GetByIdAsync(long paymentMethodId);
        Task<bool> UpdateAsync(PaymentMethod entity);
        Task<bool> PatchStatusAsync(long paymentMethodId, string status, long usersBy, long businessId);
    }
}
