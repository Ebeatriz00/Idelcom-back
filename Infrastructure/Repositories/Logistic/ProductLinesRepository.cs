using Core.Entities.Logistic;
using Core.Entities.paginations;
using Core.Interfaces.logistic;
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
    public class ProductLinesRepository(IDapperHelper dapperHelper) : IProductLinesRepository
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
                                        FROM PRODUCT_LINES
                                        WHERE DESCRIPTION LIKE '%' + @DESCRIPTION + '%'
                                          AND BUSINESS_ID = @BID
                                          AND PRODUCT_LINES_ID <> @ID
                                        """;
                var count = await _dapperHelper.ExecuteScalarAsync<int>(
                    query,
                    parameters,
                    commandType: CommandType.Text);

                return count > 0;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al validar existencia de la línea de producto.", ex.Message);
            }
        }

        public async Task<BaseResponseId> AddAsync(ProductLines entity, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    entity.BusinessId,
                    entity.Description,
                    entity.CategoriesId,
                    entity.CreateUser
                })
                .WithOutputLong("@Id")
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync(
                    "SP_WS_REGISTER_PRODUCT_LINES",
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
                throw new DatabaseException("Error al registrar la línea de producto en base de datos.", ex.Message);
            }
        }

        public async Task<PagedResult<ProductLinesItem>> GetAllAsync(long businessId, string? search, int page, int pageSize)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    BusinessId = businessId,
                    PageNumber = page,
                    PageSize = pageSize,
                    Search = search,                    
                });

                var (items, total) = await _dapperHelper.QueryPagedAsync<ProductLinesItem>(
                    "SP_WS_LIST_PRODUCT_LINES",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return new PagedResult<ProductLinesItem>
                {
                    Items = items.ToList(),
                    Page = page,
                    PageSize = pageSize,
                    Total = total
                };
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener lista de líneas de producto paginada.", ex.Message);
            }
        }

        public async Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, long? categoriesId, string? search, int page, int pageSize)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    BusinessId = businessId,
                    Search = search,
                    PageNumber = page,
                    PageSize = pageSize,
                    CategoriesId = categoriesId
                });

                var (items, total) = await _dapperHelper.QueryPagedAsync<OptionItem>("SP_WS_PRODUCT_LINES_SELECT", parameters);

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
                throw new DatabaseException("Error al obtener las líneas de producto para el selector.", ex.Message);
            }
        }

        public async Task<ProductLines?> GetByIdAsync(long productLinesId)
        {
            try
            {
                var parameters = DapperParams.From(
                   new
                   {
                       ProductLinesId = productLinesId
                   });

                return await _dapperHelper.QueryFirstOrDefaultAsync<ProductLines>("SP_WS_PRODUCT_LINES_BY_ID", parameters);
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener la línea de producto por ID.", ex.Message);
            }
        }

        public async Task<ProductLines?> GetByIdAsync(long productLinesId, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(
                   new
                   {
                       ProductLinesId = productLinesId
                   });

                return await _dapperHelper.QueryFirstOrDefaultAsync<ProductLines>("SP_WS_PRODUCT_LINES_BY_ID", parameters, transaction);
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener la línea de producto por ID.", ex.Message);
            }
        }
        public async Task<BaseResponse> UpdateAsync(ProductLines entity, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    entity.ProductLinesId,
                    entity.BusinessId,
                    entity.Description,
                    entity.CategoriesId,
                    entity.UpdateUser
                })
                 .WithOutputInt("@COutput")
                 .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync(
                    "SP_WS_UPDATE_PRODUCT_LINES",
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
                throw new DatabaseException("Error al actualizar la línea de producto en base de datos.", ex.Message);
            }
        }

        public async Task<bool> PatchStatusAsync(long productLinesId, string status, long userId, long businessId)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    ProductLinesId = productLinesId,
                    Status = status,
                    UpdateUser = userId,
                    BusinessId = businessId
                });
                var result = await _dapperHelper.QueryFirstOrDefaultAsync<dynamic>(
                    "SP_WS_UPDATE_PRODUCT_LINES_ACTIVE",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return result is not null;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar el estado de la línea de producto.", ex.Message);
            }
        }
    }
}
