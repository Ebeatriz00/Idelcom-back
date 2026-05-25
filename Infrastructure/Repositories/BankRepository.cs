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
    public class BankRepository : IBankRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;

        public BankRepository(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<bool> ExistsAsync(string description, long businessId, long? excludeId = null)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();

                string query = "SELECT COUNT(*) FROM BANK WHERE DESCRIPTION = @DESC AND BUSINESS_ID = @BID";

                if (excludeId.HasValue)
                    query += " AND BANK_ID <> @ID";

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
                throw new DatabaseException("Error al validar existencia del banco.", ex.Message);
            }
        }


        public async Task AddAsync(Bank entity)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                using var cmd = new SqlCommand("SP_WS_REGISTER_BANK", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                cmd.Parameters.AddWithValue("@DESCRIPTION", entity.Description);
                cmd.Parameters.AddWithValue("@ABRV", entity.Abrv);
                cmd.Parameters.AddWithValue("@CREATE_USER", entity.UsersBy);

                await connection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al registrar el banco en base de datos.", ex.Message);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al guardar el banco.", ex.Message);
            }
        }

        public async Task<PagedResult<Bank>> GetAllAsync(long businessId, string? search, int page, int pageSize, long? usersBy)
        {
            try
            {
                var list = new List<Bank>();
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_LIST_BANK", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
                cmd.Parameters.AddWithValue("@SEARCH", (object?)(string.IsNullOrWhiteSpace(search) ? null : search.Trim()) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PAGE", page);
                cmd.Parameters.AddWithValue("@PAGE_SIZE", pageSize);
                cmd.Parameters.AddWithValue("@CREATE_USER", (object?)usersBy ?? DBNull.Value);

                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    list.Add(new Bank
                    {
                        BankId = reader.GetInt64(0), 
                        BusinessId = reader.GetInt64(1),
                        Description = reader.GetString(2),
                        Abrv = reader.GetString(3),          
                        Status = reader.GetString(4),        
                        BankCount = reader.GetInt32(reader.GetOrdinal("BankCount")) 
                    });
                }
                int total = 0;
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    total = reader.GetInt32(0);
                }

                return new PagedResult<Bank> 
                {
                    Items = list,
                    Page = page,
                    PageSize = pageSize,
                    Total = total
                };
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener lista de bancos.", ex.Message);
            }
        }

        public async Task<PagedSelect<OptionItem>> GetBanksForSelectAsync(long businessId, string? search, int page, int pageSize)
        {
            var items = new List<OptionItem>();
            using var cn = _connectionFactory.CreateConnection();
            await cn.OpenAsync();

            using var cmd = new SqlCommand("SP_WS_BANK_SELECT", cn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 15 };
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


        public async Task<Bank> GetByIdAsync(long bankId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_BANK_BY_ID", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@BANK_ID", bankId);

                using var reader = await cmd.ExecuteReaderAsync();
                if (reader.HasRows && await reader.ReadAsync())
                {
                    return new Bank
                    {
                        BankId = reader.GetInt64(0),
                        BusinessId = reader.GetInt64(1),
                        Description = reader.GetString(2),
                        Abrv = reader.GetString(3) 
                    };
                }

                return null;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener el banco por ID.", ex.Message);
            }
        }

        public async Task<bool> UpdateAsync(Bank entity)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();
                using var cmd = new SqlCommand("SP_WS_UPDATE_BANK", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@BANK_ID", entity.BankId);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                cmd.Parameters.AddWithValue("@DESCRIPTION", entity.Description);
                cmd.Parameters.AddWithValue("@ABRV", entity.Abrv); 
                cmd.Parameters.AddWithValue("@UPDATE_USER", entity.UsersBy);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar el banco.", ex.Message);
            }
        }

        public async Task<bool> PatchStatusAsync(long bankId, string status, long usersBy, long businessId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_UPDATE_BANK_STATUS", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
                cmd.Parameters.AddWithValue("@BANK_ID", bankId);
                cmd.Parameters.AddWithValue("@UPDATE_USER", usersBy); 
                cmd.Parameters.AddWithValue("@STATUS", status);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al cambiar el estado del banco.", ex.Message);
            }
        }
    }
}
