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

namespace Infrastructure.Repositories.Logistic
{
    public class SuppliersRepository(IDapperHelper dapperHelper) : ISuppliersRepository
    {
        private readonly IDapperHelper _dapperHelper = dapperHelper;
        
        public async Task<bool> ExistsAsync(string documentNumber, string supplierName, long businessId, long? excludeId = null)
        {
            try
            {
                var parameters = DapperParams.From()
                    .WithInput("@DNUMBER", documentNumber)
                    .WithInput("@SNAME", supplierName)
                    .WithInput("@BID", businessId)
                    .WithInput("@ID", excludeId);

                const string query = """
                    SELECT COUNT(*)
                    FROM dbo.SUPPLIERS
                    WHERE 
                          SUPPLIER_NAME LIKE '%' + @SNAME + '%'
                      AND DOCUMENT_NUMBER LIKE '%' +@DNUMBER + '%'
                      AND BUSINESS_ID = @BID
                      AND (@ID IS NULL OR SUPPLIERS_ID <> @ID)
                    """;

                var count = await _dapperHelper.ExecuteScalarAsync<int>(
                    query,
                    parameters,
                    commandType: CommandType.Text);

                return count > 0;
                
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al validar existencia del proveedor.", ex.Message);
            }
        }

        public async Task<BaseResponseId> AddAsync(Suppliers entity, IDbTransaction transaction)
        {
            try
            {

                var parameters = DapperParams.From(new
                {
                    entity.BusinessId,
                    entity.SupplierTypeId,
                    entity.SupplierGroupsId,
                    entity.PaymentConditionId,
                    entity.PaymentMethodId,
                    entity.DocumentTypeId,
                    entity.DocumentNumber,
                    entity.SupplierName,
                    entity.TradeName,
                    entity.ContactName,
                    entity.Address,
                    entity.DepartmentId,
                    entity.ProvinceId,
                    entity.DistrictId,
                    entity.Phone,
                    entity.Mobile,
                    entity.Email,
                    entity.Website,
                    entity.RetainerAgent,
                    entity.PerceptionAgent,
                    entity.DetractionAgent,
                    entity.ForeignAgent,
                    entity.Observation,
                    entity.CreateUser
                })
                .WithOutputLong("@Id")
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync(
                    "SP_WS_REGISTER_SUPPLIERS",
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
                throw new DatabaseException("Error al registrar el proveedor en base de datos.", ex.Message);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al guardar el proveedor.", ex.Message);
            }
        }

        public async Task<PagedResult<SupplierItem>> GetAllAsync(long businessId, string? search, int page, int pageSize)
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

                var (items, total) = await _dapperHelper.QueryPagedAsync<SupplierItem>(
                    "SP_WS_LIST_SUPPLIERS",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return new PagedResult<SupplierItem>
                {
                    Items = items.ToList(),
                    Page = page,
                    PageSize = pageSize,
                    Total = total
                };

                
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener lista de proveedores paginada.", ex.Message);
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

                var (items, total) = await _dapperHelper.QueryPagedAsync<OptionItem>("SP_WS_SUPPLIERS_SELECT", parameters);

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
                throw new DatabaseException("Error al obtener los proveedores para el selector.", ex.Message);
            }
        }

        public async Task<Suppliers?> GetByIdAsync(long suppliersId)
        {
            try
            {
                var parameters = DapperParams.From(
                    new
                    {
                        SuppliersId = suppliersId
                    });

                return await _dapperHelper.QueryFirstOrDefaultAsync<Suppliers>("SP_WS_SUPPLIERS_BY_ID", parameters);
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener el proveedor por ID.", ex.Message);
            }
        }

        public async Task<Suppliers?> GetByIdAsync(long suppliersId, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(
                    new
                    {
                        SuppliersId = suppliersId
                    });

                return await _dapperHelper.QueryFirstOrDefaultAsync<Suppliers>("SP_WS_SUPPLIERS_BY_ID", parameters, transaction);
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener el proveedor por ID.", ex.Message);
            }
        }
        public async Task<BaseResponse> UpdateAsync(Suppliers entity, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    entity.SuppliersId,
                    entity.BusinessId,
                    entity.SupplierTypeId,
                    entity.SupplierGroupsId,
                    entity.PaymentConditionId,
                    entity.PaymentMethodId,
                    entity.DocumentTypeId,
                    entity.DocumentNumber,
                    entity.SupplierName,
                    entity.TradeName,
                    entity.ContactName,
                    entity.Address,
                    entity.DepartmentId,
                    entity.ProvinceId,
                    entity.DistrictId,
                    entity.Phone,
                    entity.Mobile,
                    entity.Email,
                    entity.Website,
                    entity.RetainerAgent,
                    entity.PerceptionAgent,
                    entity.DetractionAgent,
                    entity.ForeignAgent,
                    entity.Observation,
                    entity.UpdateUser
                })
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);


                await _dapperHelper.ExecuteAsync(
                   "SP_WS_UPDATE_SUPPLIERS",
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
                throw new DatabaseException("Error al actualizar el proveedor en base de datos.", ex.Message);
            }
        }


        public async Task<bool> PatchStatusAsync(long suppliersId, string status, long userId, long businessId)
        {
            try
            {
                var parameters = DapperParams.From()
                   .WithInput("@SUPPLERS_ID", suppliersId)
                   .WithInput("@STATUS", status)
                   .WithInput("@UPDATE_USER", userId)
                   .WithInput("@BUSINESS_ID", businessId);

                var result = await _dapperHelper.QueryFirstOrDefaultAsync<dynamic>(
                    "SP_WS_UPDATE_SUPPLIERS_ACTIVE",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return result is not null;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar el estado del proveedor.", ex.Message);
            }
        }
    }
}
