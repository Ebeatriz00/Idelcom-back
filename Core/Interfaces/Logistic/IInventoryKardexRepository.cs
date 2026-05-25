using Core.Entities.Logistic;
using SharedKernel;
using System.Data;

namespace Core.Interfaces.Logistic
{
    public interface IInventoryKardexRepository
    {
        Task<BaseResponseId> AddAsync(InventoryKardex entity, IDbTransaction transaction);
    }
}
