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
    public class CategoriesRepository(IDapperHelper dapperHelper) : ICategoriesRepository
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
                    FROM dbo.CATEGORIES
                    WHERE DESCRIPTION LIKE '%' + @DESCRIPTION + '%'
                      AND BUSINESS_ID = @BID
                      AND (@ID IS NULL OR CATEGORIES_ID <> @ID)
                    """;

                var count = await _dapperHelper.ExecuteScalarAsync<int>(
                    query,
                    parameters,
                    commandType: CommandType.Text);

                return count > 0;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al validar existencia de la categoría.", ex.Message);
            }
        }

        public async Task<BaseResponseId> AddAsync(Categories entity, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    entity.BusinessId,
                    entity.Description,
                    entity.CreateUser
                })
                .WithOutputLong("@Id")
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync(
                    "SP_WS_REGISTER_CATEGORIES",
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
                throw new DatabaseException("Error al registrar la categoría en base de datos.", ex.Message);
            }
        }

        public async Task<PagedResult<Categories>> GetAllAsync(long businessId, string? search, int page, int pageSize)
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

                var (items, total) = await _dapperHelper.QueryPagedAsync<Categories>(
                    "SP_WS_LIST_CATEGORIES",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return new PagedResult<Categories>
                {
                    Items = items.ToList(),
                    Page = page,
                    PageSize = pageSize,
                    Total = total
                };

            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener lista de categorías paginada.", ex.Message);
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

                var (items, total) = await _dapperHelper.QueryPagedAsync<OptionItem>("SP_WS_CATEGORIES_SELECT", parameters);

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
                throw new DatabaseException("Error al obtener las categorías para el selector.", ex.Message);
            }
        }

        public async Task<Categories?> GetByIdAsync(long categoriesId)
        {
            try
            {
                var parameters = DapperParams.From(
                    new
                    {
                        CategoriesId = categoriesId
                    });

                return await _dapperHelper.QueryFirstOrDefaultAsync<Categories>("SP_WS_CATEGORIES_BY_ID", parameters);
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener la categoría por ID.", ex.Message);
            }
        }

        public async Task<Categories?> GetByIdAsync(long categoriesId, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(
                    new
                    {
                        CategoriesId = categoriesId
                    });

                return await _dapperHelper.QueryFirstOrDefaultAsync<Categories>("SP_WS_CATEGORIES_BY_ID", parameters, transaction);
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener la categoría por ID.", ex.Message);
            }
        }

        public async Task<BaseResponse> UpdateAsync(Categories entity, IDbTransaction transaction)
        {
            try
            {

                var parameters = DapperParams.From(new
                {
                    entity.CategoriesId,
                    entity.BusinessId,
                    entity.Description,
                    entity.UpdateUser
                })
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync(
                    "SP_WS_UPDATE_CATEGORIES",
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
                throw new DatabaseException("Error al actualizar la categoría en base de datos.", ex.Message);
            }
        }

        public async Task<bool> PatchStatusAsync(long categoriesId, string status, long userId, long businessId)
        {
            try
            {

                var parameters = DapperParams.From()
                   .WithInput("@CATEGORIES_ID", categoriesId)
                   .WithInput("@STATUS", status)
                   .WithInput("@UPDATE_USER", userId)
                   .WithInput("@BUSINESS_ID", businessId);

                var result = await _dapperHelper.QueryFirstOrDefaultAsync<dynamic>(
                    "SP_WS_UPDATE_CATEGORIES_ACTIVE",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return result is not null;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar el estado de la categoría.", ex.Message);
            }
        }
    }
}
