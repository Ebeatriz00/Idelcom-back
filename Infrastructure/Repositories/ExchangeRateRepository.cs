using Core.Entities;
using Core.Entities.paginations;
using Core.Interfaces;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using System.Data;


namespace Infrastructure.Persistance.Repositories
{
    public class ExchangeRateRepository : IExchangeRateRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;

        public ExchangeRateRepository(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<bool> ExistsAsync(DateTime datefxrate, long businessId, long? excludeId = null)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                string query = "SELECT COUNT(*) FROM EXCHANGE_RATE WHERE DATE_FXRATE = @DATE AND BUSINESS_ID = @BID";

                if (excludeId.HasValue)
                    query += " AND EXCHANGE_RATE_ID <> @ID";

                using var cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@DATE", datefxrate.Date);
                cmd.Parameters.AddWithValue("@BID", businessId);
                if (excludeId.HasValue) cmd.Parameters.AddWithValue("@ID", excludeId.Value);

                await connection.OpenAsync();
                var count = (int)await cmd.ExecuteScalarAsync();
                return count > 0;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al validar existencia del tipo de cambio.", ex.Message);
            }
        }

        public async Task<ExchangeRate> GetLastExchangeRateAsync(long businessId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                string query = "SELECT TOP 1 EXCHANGE_RATE_ID, BUSINESS_ID, PURCHASE_TYPE, SALE_TYPE, DATE_FXRATE, STATUS FROM EXCHANGE_RATE WHERE BUSINESS_ID = @BID ORDER BY DATE_FXRATE DESC";

                using var cmd = new SqlCommand(query, cn);
                cmd.Parameters.AddWithValue("@BID", businessId);

                await cn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new ExchangeRate
                    {
                        ExchangeRateId = reader.GetInt64(0),
                        BusinessId = reader.GetInt64(1),
                        PurchaseType = reader.GetDecimal(2),
                        SaleType = reader.GetDecimal(3),
                        DateFxrate = reader.GetDateTime(4),
                        Status = reader.GetString(5)
                    };
                }
                return null;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener el último tipo de cambio.", ex.Message);
            }
        }

        public async Task AddAsync(ExchangeRate entity)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                using var cmd = new SqlCommand("SP_WS_REGISTER_EXCHANGE_RATE", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                cmd.Parameters.AddWithValue("@PURCHASE_TYPE", entity.PurchaseType);
                cmd.Parameters.AddWithValue("@SALE_TYPE", entity.SaleType);
                cmd.Parameters.AddWithValue("@DATE_FXRATE", entity.DateFxrate);
                cmd.Parameters.AddWithValue("@CREATE_USER", entity.UsersBy);

                await connection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al registrar el tipo de cambio.", ex.Message);
            }
        }

        public async Task<PagedResult<ExchangeRate>> GetAllAsync(long businessId, string? search, int page, int pageSize, long? usersBy)
        {
            try
            {
                var list = new List<ExchangeRate>();
                using var cn = _connectionFactory.CreateConnection();

                using var cmd = new SqlCommand("SP_WS_LIST_EXCHANGE_RATE", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
                cmd.Parameters.AddWithValue("@SEARCH", (object?)(string.IsNullOrWhiteSpace(search) ? null : search.Trim()) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PAGE", page);
                cmd.Parameters.AddWithValue("@PAGE_SIZE", pageSize);
                cmd.Parameters.AddWithValue("@CREATE_USER", (object?)usersBy ?? DBNull.Value);

                await cn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    // Asumiendo que el SP devuelve las columnas en este orden
                    list.Add(new ExchangeRate
                    {
                        ExchangeRateId = reader.GetInt64(0),
                        BusinessId = reader.GetInt64(1),
                        PurchaseType = reader.GetDecimal(2),
                        SaleType = reader.GetDecimal(3),
                        DateFxrate = reader.GetDateTime(4),
                        Status = reader.GetString(5),
                        ExchangeRateCount = reader.GetInt32(reader.GetOrdinal("ExchangeRateCount"))
                    });
                }

                int total = 0;
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    total = reader.GetInt32(0);
                }

                return new PagedResult<ExchangeRate>
                {
                    Items = list,
                    Page = page,
                    PageSize = pageSize,
                    Total = total
                };
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener lista de tipos de cambio.", ex.Message);
            }
        }

        public async Task<PagedSelect<OptionItem>> GetExchangeRateForSelectAsync(long businessId, string? search, int page, int pageSize)
        {
            var items = new List<OptionItem>();
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                using var cmd = new SqlCommand("SP_WS_EXCHANGE_RATE_SELECT", cn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 15 };
                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
                cmd.Parameters.AddWithValue("@SEARCH", (object?)(string.IsNullOrWhiteSpace(search) ? null : search.Trim()) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PAGE", page);
                cmd.Parameters.AddWithValue("@PAGE_SIZE", pageSize);

                await cn.OpenAsync();
                using var dr = await cmd.ExecuteReaderAsync();
                while (await dr.ReadAsync())
                {
                    items.Add(new OptionItem
                    {
                        Value = dr.GetInt64(0),
                        Label = dr.GetString(1)
                    });
                }

                var hasMore = items.Count > pageSize;
                if (hasMore) items.RemoveAt(items.Count - 1);

                return new PagedSelect<OptionItem>
                {
                    Items = items,
                    HasMore = hasMore,
                    Page = page,
                    PageSize = pageSize
                };
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener tipos de cambio para selector.", ex.Message);
            }
        }

        public async Task<ExchangeRate> GetByIdAsync(long fxrateId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                using var cmd = new SqlCommand("SP_WS_EXCHANGE_RATE_BY_ID", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@EXCHANGE_RATE_ID", fxrateId);

                await cn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                 
                    return new ExchangeRate
                    {
                        ExchangeRateId = reader.GetInt64(0),
                        BusinessId = reader.GetInt64(1),
                        PurchaseType = reader.GetDecimal(2),
                        SaleType = reader.GetDecimal(3),
                        DateFxrate = reader.GetDateTime(4),
                        Status = reader.GetString(5)
                    };
                }
                return null;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener el tipo de cambio por ID.", ex.Message);
            }
        }

        public async Task<bool> UpdateAsync(ExchangeRate exchangeRate)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();
                using var cmd = new SqlCommand("SP_WS_UPDATE_EXCHANGE_RATE", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@EXCHANGE_RATE_ID", exchangeRate.ExchangeRateId);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", exchangeRate.BusinessId);
                cmd.Parameters.AddWithValue("@PURCHASE_TYPE", exchangeRate.PurchaseType);
                cmd.Parameters.AddWithValue("@SALE_TYPE", exchangeRate.SaleType);
                cmd.Parameters.AddWithValue("@DATE_FXRATE", exchangeRate.DateFxrate);
                cmd.Parameters.AddWithValue("@UPDATE_USER", exchangeRate.UsersBy);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar el tipo de cambio.", ex.Message);
            }
        }

        public async Task<bool> PatchStatusAsync(long fxrateId, string status, long UsersBy, long businessId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();
                using var cmd = new SqlCommand("SP_WS_UPDATE_EXCHANGE_RATE_STATUS", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@EXCHANGE_RATE_ID", fxrateId);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
                cmd.Parameters.AddWithValue("@STATUS", status);
                cmd.Parameters.AddWithValue("@UPDATE_USER", UsersBy);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al cambiar el estado del tipo de cambio.", ex.Message);
            }
        }
    }
}
