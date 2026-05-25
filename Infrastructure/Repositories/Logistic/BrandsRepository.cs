using Core.Entities.Logistic;
using Core.Entities.paginations;
using Core.Interfaces.Logistic;
using DocumentFormat.OpenXml.Spreadsheet;
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
    public class BrandsRepository(IDapperHelper dapperHelper) : IBrandsRepository
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
                    FROM dbo.BRANDS
                    WHERE DESCRIPTION LIKE '%' + @DESCRIPTION + '%'
                      AND BUSINESS_ID = @BID
                      AND (@ID IS NULL OR BRANDS_ID <> @ID)
                    """;

                var count = await _dapperHelper.ExecuteScalarAsync<int>(
                    query,
                    parameters,
                    commandType: CommandType.Text);

                return count > 0;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al validar existencia de la marca.", ex.Message);
            }
        }

        public async Task<BaseResponseId> AddAsync(Brands entity, IDbTransaction transaction)
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
                    "SP_WS_REGISTER_BRANDS",
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
                throw new DatabaseException("Error al registrar la marca en base de datos.", ex.Message);
            }
        }

        public async Task<PagedResult<Brands>> GetAllAsync(long businessId, string? search, int page, int pageSize)
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

                var (items, total) = await _dapperHelper.QueryPagedAsync<Brands>(
                    "SP_WS_LIST_BRANDS",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return new PagedResult<Brands>
                {
                    Items = items.ToList(),
                    Page = page,
                    PageSize = pageSize,
                    Total = total
                };

            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener lista de marcas paginada.", ex.Message);
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

                var (items, total) = await _dapperHelper.QueryPagedAsync<OptionItem>("SP_WS_BRANDS_SELECT", parameters);

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
                throw new DatabaseException("Error al obtener las marcas para el selector.", ex.Message);
            }
        }

        public async Task<Brands?> GetByIdAsync(long brandsId)
        {
            try
            {

                var parameters = DapperParams.From(
                    new
                    {
                        BrandsId = brandsId
                    });

                return await _dapperHelper.QueryFirstOrDefaultAsync<Brands>("SP_WS_BRANDS_BY_ID", parameters);
                
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener la marca por ID.", ex.Message);
            }
        }
        public async Task<Brands?> GetByIdAsync(long brandsId, IDbTransaction transaction)
        {
            try
            {

                var parameters = DapperParams.From(
                    new
                    {
                        BrandsId = brandsId
                    });

                return await _dapperHelper.QueryFirstOrDefaultAsync<Brands>("SP_WS_BRANDS_BY_ID", parameters, transaction);

            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener la marca por ID.", ex.Message);
            }
        }

        public async Task<BaseResponse> UpdateAsync(Brands entity, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    entity.BrandsId,
                    entity.BusinessId,
                    entity.Description,
                    entity.UpdateUser
                })
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);


                await _dapperHelper.ExecuteAsync(
                   "SP_WS_UPDATE_BRANDS",
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
                throw new DatabaseException("Error al actualizar la marca en base de datos.", ex.Message);
            }
        }

        public async Task<bool> PatchStatusAsync(long brandsId, string status, long userId, long businessId)
        {
            try
            {
                var parameters = DapperParams.From()
                   .WithInput("@BRANDS_ID", brandsId)
                   .WithInput("@STATUS", status)
                   .WithInput("@UPDATE_USER", userId)
                   .WithInput("@BUSINESS_ID", businessId);

                var result = await _dapperHelper.QueryFirstOrDefaultAsync<dynamic>(
                    "SP_WS_UPDATE_BRANDS_ACTIVE",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return result is not null;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar el estado de la marca.", ex.Message);
            }
        }
    }
}
