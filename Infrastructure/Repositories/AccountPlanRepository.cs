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
    public class AccountPlanRepository : IAccountPlanRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;
        public AccountPlanRepository(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<bool> ExistsAsync(string code, long businessId, long? excludeId = null)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                string query = "SELECT COUNT(*) FROM ACCOUNT_PLAN WHERE ACCOUNT_CODE = @CODE AND BUSINESS_ID = @BID";

                if (excludeId.HasValue)
                    query += " AND ACCOUNT_PLAN_ID <> @ID";

                using var cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@CODE", code);
                cmd.Parameters.AddWithValue("@BID", businessId);
                if (excludeId.HasValue) cmd.Parameters.AddWithValue("@ID", excludeId.Value);

                await connection.OpenAsync();
                var count = (int)await cmd.ExecuteScalarAsync();
                return count > 0;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al validar existencia del tipo comprobante de pago.", ex.Message);
            }
        }
        public async Task AddAsync(AccountPlan entity)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                using var cmd = new SqlCommand("SP_WS_REGISTER_ACCOUNT_PLAN", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                cmd.Parameters.AddWithValue("@ACCOUNT_CODE", entity.AccountCode);
                cmd.Parameters.AddWithValue("@DESCRIPTION", entity.AccountName);
                cmd.Parameters.AddWithValue("@ACCOUNT_TYPE_ID", (object?)entity.AccountTypeId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ACCOUNT_LEVEL_ID", (object?)entity.accountLevelId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TYPE_ANALYSIS_ID", (object?)entity.TypeAnalysisId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CURRENCY_ID", (object?)entity.CurrencyId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@AUXILIARY_TYPE_ID", (object?)entity.AuxiliaryTypeId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@DIFFERENCE_CHANGE", (object?)entity.DiferenceChange ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@DOC_CONTROL", (object?)entity.DocControl ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ACCOUNT_AMARRE_DEBIT", (object?)entity.AccountAmarreDebit ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ACCOUNT_CREDIT_ACCOUNT", (object?)entity.AccountAmarreCredit ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CREATE_USER", entity.UsersBy);

                await connection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al registrar el tipo de comprobante de pago en base de datos.", ex.Message);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al guardar el tipo de comprobante de pago.", ex.Message);
            }
        }
        public async Task<PagedResult<AccountPlan>> GetAllAsync(long businessId, string? search, int page, int pageSize, long? usersBy)
        {
            try
            {
                var list = new List<AccountPlan>();
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_LIST_ACCOUNT_PLAN", cn)
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
                    list.Add(new AccountPlan
                    {
                        AccountPlanId = reader.GetInt64(0),
                        BusinessId = reader.GetInt64(1),
                        AccountCode = reader.GetString(2),
                        AccountName = reader.GetString(3),
                        AccountType = reader.GetString(4),
                        Status = reader.GetString(5),
                        AccountPlanCount = reader.GetInt32(6),
                    });
                }
                int total = 0;
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    total = reader.GetInt32(0);
                }

                return new PagedResult<AccountPlan>
                {
                    Items = list,
                    Page = page,
                    PageSize = pageSize,
                    Total = total
                };
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener lista del tipo de comprobante de pago.", ex.Message);
            }
        }
        public async Task<PagedSelect<OptionItem>> GetAccountPlanForSelectAsync(long businessId, string? search, int page, int pageSize)
        {
            var items = new List<OptionItem>();
            using var cn = _connectionFactory.CreateConnection();
            await cn.OpenAsync();

            using var cmd = new SqlCommand("SP_WS_ACCOUNT_PLAN_SELECT", cn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 15 };
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
        public async Task<AccountPlan> GetByIdAsync(long AccountPlanId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_ACCOUNT_PLAN_BY_ID", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@ACCOUNT_PLAN_ID", AccountPlanId);

                using var reader = await cmd.ExecuteReaderAsync();
                if (reader.HasRows && await reader.ReadAsync())
                {
                    return new AccountPlan
                    {
                        AccountPlanId = reader.GetInt64(0),
                        BusinessId = reader.GetInt64(1),
                        AccountCode = reader.GetString(2),
                        AccountName = reader.GetString(3),
                        AccountTypeId = reader.IsDBNull(4) ? null : reader.GetInt32(4),
                        accountLevelId = reader.IsDBNull(5) ? null : reader.GetInt32(5),
                        TypeAnalysisId = reader.IsDBNull(6) ? null : reader.GetInt32(6),
                        CurrencyId = reader.IsDBNull(7) ? null : reader.GetInt64(7),
                        AuxiliaryTypeId = reader.IsDBNull(8) ? null : reader.GetInt32(8),
                        DiferenceChange = reader.IsDBNull(9) ? null : reader.GetString(9),
                        DocControl = reader.IsDBNull(10) ? null : reader.GetString(10),
                        AccountAmarreDebit = reader.IsDBNull(11) ? null : reader.GetInt64(11),
                        AccountAmarreCredit = reader.IsDBNull(12) ? null : reader.GetInt64(12),

                    };
                }

                return null;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener el tipo de comprobante de pago por ID.", ex.Message);
            }
        }
        public async Task<bool> UpdateAsync(AccountPlan entity)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_UPDATE_ACCOUNT_PLAN", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@ACCOUNT_PLAN_ID", entity.AccountPlanId);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                cmd.Parameters.AddWithValue("@ACCOUNT_CODE", entity.AccountCode);
                cmd.Parameters.AddWithValue("@DESCRIPTION", entity.AccountName);
                cmd.Parameters.AddWithValue("@ACCOUNT_TYPE_ID", (object?)entity.AccountTypeId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ACCOUNT_LEVEL_ID", (object?)entity.accountLevelId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TYPE_ANALYSIS_ID", (object?)entity.TypeAnalysisId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CURRENCY_ID", (object?)entity.CurrencyId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@AUXILIARY_TYPE_ID", (object?)entity.AuxiliaryTypeId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@DIFFERENCE_CHANGE", (object?)entity.DiferenceChange ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@DOC_CONTROL", (object?)entity.DocControl ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ACCOUNT_AMARRE_DEBIT", (object?)entity.AccountAmarreDebit ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ACCOUNT_CREDIT_ACCOUNT", (object?)entity.AccountAmarreCredit ?? DBNull.Value);
                
                cmd.Parameters.AddWithValue("@UPDATE_USER", entity.UsersBy);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar el tipo de comprobante de pago.", ex.Message);
            }
        }
        public async Task<bool> PatchStatusAsync(long AccountPlanId, string status, long UsersBy, long businessId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_UPDATE_ACCOUNT_PLAN_ACTIVE", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
                cmd.Parameters.AddWithValue("@ACCOUNT_PLAN_ID", AccountPlanId);
                cmd.Parameters.AddWithValue("@UPDATE_USER", UsersBy);
                cmd.Parameters.AddWithValue("@STATUS", status);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al cambiar el estado del tipo de comprobante de pago.", ex.Message);
            }
        }

    }
}
