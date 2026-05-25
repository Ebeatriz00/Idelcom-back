using Core.Entities.Logistic;
using Core.Entities.paginations;
using Core.Interfaces.Logistic;
using Core.Projections.Logistic;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using SharedKernel;
using System.Data;

namespace Infrastructure.Repositories.Logistic
{
    public class ProductTypesRepository(IDapperHelper dapperHelper) : IProductTypesRepository
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
                    FROM dbo.PRODUCT_TYPES
                    WHERE DESCRIPTION LIKE '%' + @DESCRIPTION + '%'
                      AND BUSINESS_ID = @BID
                      AND (@ID IS NULL OR PRODUCT_TYPES_ID <> @ID)
                    """;

                var count = await _dapperHelper.ExecuteScalarAsync<int>(
                    query,
                    parameters,
                    commandType: CommandType.Text);

                return count > 0;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al validar existencia del tipo de producto.", ex.Message);
            }
        }

        public async Task<BaseResponseId> AddAsync(ProductTypes entity, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    entity.BusinessId,
                    entity.Description,
                    entity.IsConsumable,
                    entity.IsReturnable,
                    entity.RequiresSerial,
                    entity.CreateUser
                })
                .WithOutputLong("@Id")
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync(
                    "SP_WS_REGISTER_PRODUCT_TYPES",
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
                throw new DatabaseException("Error al registrar el tipo de producto en base de datos.", ex.Message);
            }
        }

        public async Task<PagedResult<ProductTypeItem>> GetAllAsync(long businessId, string? search, int page, int pageSize)
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

                var (items, total) = await _dapperHelper.QueryPagedAsync<ProductTypeItem>(
                    "SP_WS_LIST_PRODUCT_TYPES",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return new PagedResult<ProductTypeItem>
                {
                    Items = items.ToList(),
                    Page = page,
                    PageSize = pageSize,
                    Total = total
                };
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener lista de tipos de producto paginada.", ex.Message);
            }
        }

        public async Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, string? search, int page, int pageSize)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    BusinessId = businessId,
                    PageNumber = page,
                    Search = search,
                    PageSize = pageSize

                });

                var (items, total) = await _dapperHelper.QueryPagedAsync<OptionItem>("SP_WS_PRODUCT_TYPES_SELECT",parameters);

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
                throw new DatabaseException("Error al obtener los tipos de producto para el selector.", ex.Message);
            }
        }

        public async Task<ProductTypes?> GetByIdAsync(long productTypesId)
        {
            try
            {
                var parameters = DapperParams.From(
                    new
                    {
                        ProductTypesId = productTypesId 
                    });

                return await _dapperHelper.QueryFirstOrDefaultAsync<ProductTypes>( "SP_WS_PRODUCT_TYPES_BY_ID", parameters);
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener el tipo de producto por ID.", ex.Message);
            }
        }

        public async Task<ProductTypes?> GetByIdAsync(long productTypesId, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(
                    new
                    {
                        ProductTypesId = productTypesId
                    });

                return await _dapperHelper.QueryFirstOrDefaultAsync<ProductTypes>("SP_WS_PRODUCT_TYPES_BY_ID", parameters, transaction);
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener el tipo de producto por ID.", ex.Message);
            }
        }

        public async Task<BaseResponse> UpdateAsync(ProductTypes entity, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    entity.ProductTypesId,
                    entity.Description,
                    entity.IsConsumable,
                    entity.IsReturnable,
                    entity.RequiresSerial,
                    entity.UpdateUser
                })
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                 await _dapperHelper.ExecuteAsync(
                    "SP_WS_UPDATE_PRODUCT_TYPES",
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
                throw new DatabaseException("Error al actualizar el tipo de producto en base de datos.", ex.Message);
            }
        }

        public async Task<bool> PatchStatusAsync(long productTypesId, string status, long usersBy, long businessId)
        {
            try
            {
                var parameters = DapperParams.From()
                    .WithInput("@PRODUCT_TYPES_ID", productTypesId)
                    .WithInput("@STATUS", status)
                    .WithInput("@UPDATE_USER", usersBy)
                    .WithInput("@BUSINESS_ID", businessId);

                var result = await _dapperHelper.QueryFirstOrDefaultAsync<dynamic>(
                    "SP_WS_UPDATE_PRODUCT_TYPES_ACTIVE",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return result is not null;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar el estado del tipo de producto.", ex.Message);
            }
        }
    }
}
