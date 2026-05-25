using Core.Entities.Logistic;
using Core.Entities.paginations;
using Core.Interfaces.Logistic;
using Core.Interfaces.Services;
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
    public class ProductsRepository(IDapperHelper dapperHelper) : IProductsRepository
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
                    FROM dbo.PRODUCTS
                    WHERE DESCRIPTION LIKE '%' + @DESCRIPTION + '%'
                      AND BUSINESS_ID = @BID
                      AND (@ID IS NULL OR PRODUCTS_ID <> @ID)
                    """;

                var count = await _dapperHelper.ExecuteScalarAsync<int>(
                    query,
                    parameters,
                    commandType: CommandType.Text);

                return count > 0;                
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al validar existencia del producto.", ex.Message);
            }
        }

        public async Task<BaseResponseId> AddAsync(Products entity, IDbTransaction transaction)
        {
            try
            {
                    var parameters = DapperParams.From(new
                    {
                        entity.BusinessId,
                        entity.Sku,
                        entity.Barcode,
                        entity.PartNum,
                        entity.Description,
                        entity.ShortDescription,
                        entity.ProductTypeId,
                        entity.ProductLinesId,
                        entity.CategoriesId,
                        entity.BrandsId,
                        entity.UomId,
                        entity.SunatId,
                        entity.StockMin,
                        entity.StockMax,
                        entity.ConversionFactor,
                        entity.IsActive,
                        entity.IsStockable,
                        entity.IsServices,
                        entity.IsReturnable,
                        entity.IsTool,
                        entity.CanBuy,
                        entity.CanSell,
                        entity.ManageLots,
                        entity.ManegesSerials,
                        entity.ExpirationControl,
                        entity.Weight,
                        entity.Volume,
                        entity.Files,
                        entity.CreateUser
                    })
                    .WithOutputLong("@Id")
                    .WithOutputInt("@COutput")
                    .WithOutputString("@SOutput", 500);

                    await _dapperHelper.ExecuteAsync(
                        "SP_WS_REGISTER_PRODUCTS",
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
                throw new DatabaseException("Error al registrar el producto en base de datos.", ex.Message);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al guardar el producto.", ex.Message);
            }
        }

        public async Task<PagedResult<ProductsItem>> GetAllAsync(long businessId, long? categoriesId, long? productTypeId, long? brandsId, string? search, int page, int pageSize)
        {
            try
            {

                var parameters = DapperParams.From(new
                {
                    BusinessId = businessId,
                    CategoriesId = categoriesId,
                    ProductTypeId = productTypeId,
                    BrandsId = brandsId,    
                    Search = search,
                    PageNumber = page,
                    PageSize = pageSize
                });

                var (items, total) = await _dapperHelper.QueryPagedAsync<ProductsItem>(
                    "SP_WS_LIST_PRODUCTS",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return new PagedResult<ProductsItem>
                {
                    Items = items.ToList(),
                    Page = page,
                    PageSize = pageSize,
                    Total = total
                };


                
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener lista de productos paginada.", ex.Message);
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

                var (items, total) = await _dapperHelper.QueryPagedAsync<OptionItem>("SP_WS_PRODUCTS_SELECT", parameters);

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
                throw new DatabaseException("Error al obtener los productos para el selector.", ex.Message);
            }
        }

        public async Task<Products?> GetByIdAsync(long productsId)
        {
            try
            {
                var parameters = DapperParams.From(
                    new
                    {
                        ProductsId = productsId
                    });

                return await _dapperHelper.QueryFirstOrDefaultAsync<Products>("SP_WS_PRODUCT_BY_ID", parameters);

                
               
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener el producto por ID.", ex.Message);
            }
        }

        public async Task<Products?> GetByIdAsync(long productsId, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(
                    new
                    {
                        ProductsId = productsId
                    });

                return await _dapperHelper.QueryFirstOrDefaultAsync<Products>("SP_WS_PRODUCT_BY_ID", parameters, transaction);



            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener el producto por ID.", ex.Message);
            }
        }
        public async Task<BaseResponse> UpdateAsync(Products entity, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    entity.ProductsId,
                    entity.BusinessId,
                    entity.Sku,
                    entity.Barcode,
                    entity.PartNum,
                    entity.Description,
                    entity.ShortDescription,
                    entity.ProductTypeId,
                    entity.ProductLinesId,
                    entity.CategoriesId,
                    entity.BrandsId,
                    entity.UomId,
                    entity.SunatId,
                    entity.StockMin,
                    entity.StockMax,
                    entity.ConversionFactor,
                    entity.IsActive,
                    entity.IsStockable,
                    entity.IsServices,
                    entity.IsReturnable,
                    entity.IsTool,
                    entity.CanBuy,
                    entity.CanSell,
                    entity.ManageLots,
                    entity.ManegesSerials,
                    entity.ExpirationControl,
                    entity.Weight,
                    entity.Volume,
                    entity.Files,
                    entity.UpdateUser
                })
                    .WithOutputInt("@COutput")
                    .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync(
                    "SP_WS_UPDATE_PRODUCTS",
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
                throw new DatabaseException("Error al actualizar el producto en base de datos.", ex.Message);
            }
        }

        public async Task<bool> PatchStatusAsync(long productsId, string status, long userId, long businessId)
        {
            try
            {
                var parameters = DapperParams.From()
                   .WithInput("@PRODUCTS_ID", productsId)
                   .WithInput("@STATUS", status)
                   .WithInput("@UPDATE_USER", userId)
                   .WithInput("@BUSINESS_ID", businessId);

                var result = await _dapperHelper.QueryFirstOrDefaultAsync<dynamic>(
                    "SP_WS_UPDATE_PRODUCTS_ACTIVE",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return result is not null;

                
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar el estado del producto.", ex.Message);
            }
        }
    }
}
