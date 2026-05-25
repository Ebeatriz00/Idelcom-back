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
    public class StatePreSaleRepository : IStatePreSaleRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;

        public StatePreSaleRepository(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<bool> ExistsAsync(string description, long businessId, long? excludeId = null)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                string query = "SELECT COUNT(*) FROM STATE_PRE_SALE WHERE STATE_DESC = @DESC AND BUSINESS_ID = @BID";

                if (excludeId.HasValue)
                    query += " AND STATE_PRE_SALE_ID <> @ID";

                using var cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@DESC", description);
                cmd.Parameters.AddWithValue("@BID", businessId);
                if (excludeId.HasValue) cmd.Parameters.AddWithValue("@ID", excludeId.Value);

                await connection.OpenAsync();
                var count = (int)await cmd.ExecuteScalarAsync();
                return count > 0;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al validar existencia del estado de preventa.", ex.Message);
            }
        }

        public async Task AddAsync(StatePreSale entity)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                using var cmd = new SqlCommand("SP_WS_REGISTER_STATE_PRE_SALE", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                cmd.Parameters.AddWithValue("@STATE_COLOR", entity.StateColor);
                cmd.Parameters.AddWithValue("@STATE_DESC", entity.StateDesc);
                cmd.Parameters.AddWithValue("@NUM_PERC_PRO", entity.NumPercPro);
                cmd.Parameters.AddWithValue("@NUM_ORDER", entity.NumOrder);
                cmd.Parameters.AddWithValue("@CREATE_USER", entity.UsersBy); 

                await connection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al registrar estado de preventa en base de datos.", ex.Message);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al guardar estado de preventa.", ex.Message);
            }
        }

        public async Task<PagedResult<StatePreSale>> GetAllAsync(long businessId, string? search, int page, int pageSize)
        {
            try
            {
                var list = new List<StatePreSale>();
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_LIST_STATE_PRE_SALE", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
                cmd.Parameters.AddWithValue("@SEARCH", (object?)(string.IsNullOrWhiteSpace(search) ? null : search.Trim()) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PAGE", page);
                cmd.Parameters.AddWithValue("@PAGE_SIZE", pageSize);

                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    list.Add(new StatePreSale
                    {
                        StatePreSaleId = reader.GetInt64(0),
                        BusinessId = reader.GetInt64(1),
                        StateDesc = reader.GetString(2),
                        NumPercPro = reader.GetInt32(3),
                        Status = reader.GetString(4)
                    });
                }
                int total = 0;
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    total = reader.GetInt32(0);
                }

                return new PagedResult<StatePreSale>
                {
                    Items = list,
                    Page = page,
                    PageSize = pageSize,
                    Total = total
                };
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener lista de estados de preventa.", ex.Message);
            }
        }

        public async Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, string? search, int page, int pageSize)
        {
            var items = new List<OptionItem>();
            using var cn = _connectionFactory.CreateConnection();
            await cn.OpenAsync();

            using var cmd = new SqlCommand("SP_WS_STATE_PRE_SALE_SELECT", cn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 15 };
            cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
            cmd.Parameters.AddWithValue("@SEARCH", (object?)(string.IsNullOrWhiteSpace(search) ? null : search.Trim()) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PAGE", page);
            cmd.Parameters.AddWithValue("@PAGE_SIZE", pageSize);

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

        public async Task<StatePreSale> GetByIdAsync(long statePreSaleId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_STATE_PRE_SALE_BY_ID", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@STATE_PRE_SALE_ID", statePreSaleId);

                using var reader = await cmd.ExecuteReaderAsync();
                if (reader.HasRows && await reader.ReadAsync())
                {
                    return new StatePreSale
                    {
                        StatePreSaleId = reader.GetInt64(0),
                        BusinessId = reader.GetInt64(1),
                        StateColor = reader.GetString(2),
                        StateDesc = reader.GetString(3),
                        NumPercPro = reader.GetInt32(4),
                        NumOrder = reader.GetInt32(5)
                    };
                }

                return null;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener estado de preventa por ID.", ex.Message);
            }
        }

        public async Task<bool> UpdateAsync(StatePreSale entity)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_UPDATE_STATE_PRE_SALE", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@STATE_PRE_SALE_ID", entity.StatePreSaleId);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                cmd.Parameters.AddWithValue("@STATE_COLOR", entity.StateColor);
                cmd.Parameters.AddWithValue("@STATE_DESC", entity.StateDesc);
                cmd.Parameters.AddWithValue("@NUM_PERC_PRO", entity.NumPercPro);
                cmd.Parameters.AddWithValue("@NUM_ORDER", entity.NumOrder);
                cmd.Parameters.AddWithValue("@UPDATE_USER", entity.UsersBy); 

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar estado de preventa.", ex.Message);
            }
        }

        public async Task<bool> PatchStatusAsync(long statePreSaleId, string status, long UsersBy, long businessId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_UPDATE_STATE_PRE_SALE_ACTIVE", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
                cmd.Parameters.AddWithValue("@STATE_PRE_SALE_ID", statePreSaleId);
                cmd.Parameters.AddWithValue("@UPDATE_USER", UsersBy);
                cmd.Parameters.AddWithValue("@STATUS", status);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al cambiar el estado de preventa.", ex.Message);
            }
        }
    }
}
