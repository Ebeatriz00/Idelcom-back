using Core.Entities;
using Core.Entities.Logistic;
using Core.Entities.paginations;
using Core.Interfaces.Logistic;
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
    public class FileTrackingProductsRepository(IDapperHelper dapperHelper) : IFileTrackingProductsRepository
    {
        private readonly IDapperHelper _dapperHelper = dapperHelper;

        public async Task<bool> ExistsAsync(long businessId, long productsId, string fileName, long? excludeId = null)
        {
            try
            {
                var parameters = DapperParams.From()
                    .WithInput("@DESCRIPTION", fileName)
                    .WithInput("@BID", businessId)
                    .WithInput("@PID", productsId)
                    .WithInput("@ID", excludeId);

                const string query = """
                    SELECT COUNT(*)
                    FROM dbo.FILE_TRACKING_PRODUCTS
                    WHERE FILE_TITLE LIKE '%' + @DESCRIPTION + '%'
                      AND PRODUCTS_ID = @PID
                      AND BUSINESS_ID = @BID
                      AND (@ID IS NULL OR FILE_TRACKING_PRODUCTS_ID <> @ID)
                    """;

                var count = await _dapperHelper.ExecuteScalarAsync<int>(
                    query,
                    parameters,
                    commandType: CommandType.Text);

                return count > 0;
                
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al validar existencia del archivo.", ex.Message);
            }
        }

        public async Task<BaseResponseId> AddAsync(FileTrackingProducts entity, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    entity.BusinessId,
                    entity.ProductsId,
                    entity.FileTitle,
                    entity.FileUrl,
                    entity.RelativePath,
                    entity.CreateUser
                })
                .WithOutputLong("@Id")
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync(
                    "SP_WS_REGISTER_FILE_TRACKING_PRODUCTS",
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
            catch (Exception ex)
            {
                throw new DatabaseException("Error al registrar los archivos del producto.", ex.Message);
            }
        }

        public async Task<PagedResult<FileTrackingProducts>> GetAllAsync(long businessId, long productsId, int page, int pageSize)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    BusinessId = businessId,
                    ProductsId = productsId,
                    PageNumber = page,
                    PageSize = pageSize
                });

                var (items, total) = await _dapperHelper.QueryPagedAsync<FileTrackingProducts>(
                    "SP_WS_LIST_FILE_TRACKING_PRODUCTS",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return new PagedResult<FileTrackingProducts>
                {
                    Items = items.ToList(),
                    Page = page,
                    PageSize = pageSize,
                    Total = total
                };
               
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener la lista de archivos paginada.", ex.Message);
            }
        }

        public async Task<BaseResponse> DeleteAsync(long fileId, long productsId, long businessId,  IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    FileTrackingProductsId = fileId,
                    ProductsId = productsId,
                    BusinessId = businessId,
                })
               .WithOutputInt("@COutput")
               .WithOutputString("@SOutput", 500);


                await _dapperHelper.ExecuteAsync(
                   "SP_WS_DELETE_FILE_TRACKING_PRODUCTS",
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
                throw new DatabaseException("Error al eliminar el archivo.", ex.Message);
            }
        }
    }
}
