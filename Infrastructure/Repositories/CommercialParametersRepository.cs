using Core.Entities;
using Core.Entities.paginations;
using Core.Interfaces;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class CommercialParametersRepository : ICommercialParametersRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;
        public CommercialParametersRepository(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<bool> ExistsAsync(string description, long businessId, long? excludeId = null)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();

                var baseQuery = new StringBuilder("SELECT COUNT(*) FROM COMMERCIAL_PARAMETERS WHERE PARAMETERS_NAME = @NAME AND BUSINESS_ID = @BID");

                if (excludeId.HasValue)
                    baseQuery.Append(" AND PARAMETERS_COMM_ID <> @ID");

                using var cmd = new SqlCommand(baseQuery.ToString(), connection);
                cmd.Parameters.AddWithValue("@NAME", description);
                cmd.Parameters.AddWithValue("@BID", businessId);
                if (excludeId.HasValue)
                    cmd.Parameters.AddWithValue("@ID", excludeId.Value);

                await connection.OpenAsync();
                var count = (int)await cmd.ExecuteScalarAsync();
                return count > 0;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al validar existencia de parametros comerciales.", ex.Message);
            }
        }

        public async Task AddAsync(CommercialParameters entity)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                using var cmd = new SqlCommand("SP_WS_REGISTER_COMMERCIAL_PARAMETERS", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                cmd.Parameters.AddWithValue("@PARAMETERS_NAME",entity.ParametersName) ;
                cmd.Parameters.AddWithValue("@MIN_VALUE", (object?)entity.MinValue ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PARAMETERS_VALUE", entity.ParametersValue);
                cmd.Parameters.AddWithValue("@CREATE_USER", entity.UsersBy);

                await connection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al registrar el área en base de datos.", ex.Message);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al guardar el área.", ex.Message);
            }
        }

        public async Task<PagedResult<CommercialParameters>> GetAllAsync(long businessId, string? search, int page, int pageSize, long? usersBy )
        {
            try
            {
                var list = new List<CommercialParameters>();
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_LIST_COMMERCIAL_PARAMETERS", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
                cmd.Parameters.AddWithValue("@SEARCH", (object?)(string.IsNullOrWhiteSpace(search) ? null : search.Trim()) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CREATE_USER", (object?)(usersBy.HasValue ? usersBy.Value : null) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PAGE", page);
                cmd.Parameters.AddWithValue("@PAGE_SIZE", pageSize);

                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    list.Add(new CommercialParameters
                    {
                        BusinessId = reader.GetInt64(0),
                        CommercialParametersId = reader.GetInt32(1),
                        ParametersName = reader.GetString(2),
                        ParametersValue = reader.GetInt32(3),
                        Status = reader.GetString(4),
                        MinValue = reader.IsDBNull(5) ? null : reader.GetInt32(5)
                    });
                }

                int total = 0;
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    total = reader.GetInt32(0);
                }

                return new PagedResult<CommercialParameters>
                {
                    Items = list,
                    Page = page,
                    PageSize = pageSize,
                    Total = total
                };
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener lista de áreas paginada.", ex.Message);
            }
        }

        public async Task<CommercialParameters> GetByIdAsync(int commercialParametersId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();
                using var cmd = new SqlCommand("SP_WS_COMMERCIAL_PARAMETERS_BY_ID", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@PARAMETERS_COMM_ID", commercialParametersId);
                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new CommercialParameters
                    {
                        CommercialParametersId = reader.GetInt32(0),
                        BusinessId = reader.GetInt64(1),
                        ParametersName = reader.GetString(2),
                        ParametersValue = reader.GetInt32(3),
                        Status = reader.GetString(4),
                        MinValue = reader.IsDBNull(5) ? null : reader.GetInt32(5)
                    };
                }
                return null;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener el área por ID.", ex.Message);
            }
        }

        public async Task<bool> UpdateAsync(CommercialParameters comm)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_UPDATE_COMMERCIAL_PARAMETERS", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@PARAMETERS_COMM_ID", comm.CommercialParametersId);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", comm.BusinessId);
                cmd.Parameters.AddWithValue("@PARAMETERS_NAME", comm.ParametersName);
                cmd.Parameters.AddWithValue("@MIN_VALUE", (object?)comm.MinValue ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PARAMETERS_VALUE", comm.ParametersValue);
                cmd.Parameters.AddWithValue("@UPDATE_USER", comm.UsersBy);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar el área en base de datos.", ex.Message);
            }
        }

        public async Task<bool> PatchStatusAsync(int commercialParametersId, string status, long updatedBy, long businessId)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                using var cmd = new SqlCommand("SP_WS_UPDATE_COMMERCIAL_PARMETERS_STATUS", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@PARAMETERS_COMM_ID", commercialParametersId);
                cmd.Parameters.AddWithValue("@STATUS", status);
                cmd.Parameters.AddWithValue("@UPDATE_USER", updatedBy);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);

                await connection.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar el estado del área.", ex.Message);
            }
        }

        
    }
}
