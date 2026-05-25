using Core.Entities.Logistic;
using Core.Interfaces.Logistic;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using SharedKernel;
using System.Data;

namespace Infrastructure.Repositories.Logistic
{
    public class InventoryKardexRepository(IDapperHelper dapperHelper) : IInventoryKardexRepository
    {
        private readonly IDapperHelper _dapperHelper = dapperHelper;

        public async Task<BaseResponseId> AddAsync(InventoryKardex entity, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    entity.BusinessId,
                    entity.WarehouseId,
                    entity.ProductsId,
                    entity.WareHouseMovementId,
                    entity.WareHouseMovementDetailId,
                    entity.MovementTypesId,
                    entity.MovementDate,
                    entity.EntryQuantity,
                    entity.ExitQuantity,
                    entity.PreviousStock,
                    entity.FinalStock,
                    entity.UnitCost,
                    entity.AverageCost,
                    entity.TotalCost,
                    entity.ReferenceDocumentType,
                    entity.ReferenceDocumentNumber,
                    entity.Observation,
                    entity.CreateUser
                })
                .WithOutputLong("@Id")
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync(
                    "SP_WS_REGISTER_INVENTORY_KARDEX",
                    parameters,
                    transaction);

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
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al registrar kardex de inventario en base de datos.", ex.Message);
            }
        }
    }
}
