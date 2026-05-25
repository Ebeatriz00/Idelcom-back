using Core.Entities.Logistic;
using Core.Entities.paginations;
using Core.Interfaces.Logistic;
using Core.Projections.Logistic;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Infrastructure.Repositories.Logistic
{
    public class WarehousesRepository(IDapperHelper dapperHelper) : IWarehousesRepository
    {
        private readonly IDapperHelper _dapperHelper = dapperHelper;

        public async Task<bool> ExistsAsync(string description, long businessId, long? excludeId = null)
        {
            try
            {
                var parameters = DapperParams.From()
                    .WithInput("@DESCRIPTION", description)
                    .WithInput("@BID", businessId)
                    .WithInput("@ID", excludeId);

                const string query = """
                    SELECT COUNT(*)
                    FROM dbo.WAREHOUSES
                    WHERE DESCRIPTION LIKE '%' + @DESCRIPTION + '%'
                      AND BUSINESS_ID = @BID
                      AND (@ID IS NULL OR WAREHOUSES_ID <> @ID)
                    """;

                var count = await _dapperHelper.ExecuteScalarAsync<int>(
                    query,
                    parameters,
                    commandType: CommandType.Text);

                return count > 0;                
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al validar existencia del almacén.", ex.Message);
            }
        }

        public async Task<BaseResponseId> AddAsync(Warehouses entity, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    entity.BusinessId,
                    entity.Description,
                    entity.Address,
                    entity.DepartmentId,
                    entity.ProvinceId,
                    entity.DistrictId,
                    entity.CreateUser
                })
                .WithOutputLong("@Id")
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync(
                    "SP_WS_REGISTER_WAREHOUSES",
                    parameters
                    , transaction);

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
                throw new DatabaseException("Error al registrar el almacén en base de datos.", ex.Message);
            }
        }

        public async Task<PagedResult<WarehousesItem>> GetAllAsync(long businessId, string? search, int page, int pageSize)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    BusinessId = businessId,
                    Search = search,
                    PageNumber = page,
                    PageSize = pageSize
                });

                var (items, total) = await _dapperHelper.QueryPagedAsync<WarehousesItem>(
                    "SP_WS_LIST_WAREHOUSES",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return new PagedResult<WarehousesItem>
                {
                    Items = items.ToList(),
                    Page = page,
                    PageSize = pageSize,
                    Total = total
                };

                
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener lista de almacenes paginada.", ex.Message);
            }
        }

        public async Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, string? search, int page, int pageSize)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    BusinessId = businessId,
                    Search = search,
                    PageNumber = page,
                    PageSize = pageSize
                });

                var (items, total) = await _dapperHelper.QueryPagedAsync<OptionItem>("SP_WS_WAREHOUSES_SELECT", parameters);

                return new PagedSelect<OptionItem>
                {
                    Items = items.ToList(),
                    Page = page,
                    PageSize = pageSize,
                    HasMore = false
                };

            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener los almacenes para el selector.", ex.Message);
            }
        }

        public async Task<Warehouses?> GetByIdAsync(long warehousesId)
        {
            try
            {
                var parameters = DapperParams.From(
                    new
                    {
                        WarehousesId = warehousesId
                    });

                return await _dapperHelper.QueryFirstOrDefaultAsync<Warehouses>("SP_WS_WAREHOUSES_BY_ID", parameters);

                
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener el almacén por ID.", ex.Message);
            }
        }

        public async Task<Warehouses?> GetByIdAsync(long warehousesId, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(
                    new
                    {
                        WarehousesId = warehousesId
                    });

                return await _dapperHelper.QueryFirstOrDefaultAsync<Warehouses>("SP_WS_WAREHOUSES_BY_ID", parameters, transaction);


            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener el almacén por ID.", ex.Message);
            }
        }


        public async Task<BaseResponse> UpdateAsync(Warehouses entity, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    entity.WarehousesId,
                    entity.BusinessId,
                    entity.Description,
                    entity.Address,
                    entity.DepartmentId,
                    entity.ProvinceId,
                    entity.DistrictId,
                    entity.UpdateUser
                })
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);


                await _dapperHelper.ExecuteAsync(
                   "SP_WS_UPDATE_WAREHOUSES",
                   parameters, transaction);

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
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar el almacén en base de datos.", ex.Message);
            }
        }

        public async Task<bool> PatchStatusAsync(long warehousesId, string status, long userId, long businessId)
        {
            try
            {
                var parameters = DapperParams.From()
                   .WithInput("@WAREHOUSES_ID", warehousesId)
                   .WithInput("@STATUS", status)
                   .WithInput("@UPDATE_USER", userId)
                   .WithInput("@BUSINESS_ID", businessId);

                var result = await _dapperHelper.QueryFirstOrDefaultAsync<dynamic>(
                    "SP_WS_UPDATE_WAREHOUSES_ACTIVE",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return result is not null;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar el estado del almacén.", ex.Message);
            }
        }
    }
}
