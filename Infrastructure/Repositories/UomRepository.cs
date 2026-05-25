
using Core.Entities;
using Core.Entities.paginations;
using Core.Interfaces;
using Infrastructure.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class UomRepository : IUomRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;

        public UomRepository(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<bool> ExistsAsync(string description, string codeSunat, long businessId, long? excludeId = null)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                string query = "SELECT COUNT(*) FROM UNIT_OF_MEASURE WHERE DESCRIPTION = @DESC AND CODE_SUNAT = @CODE AND BUSINESS_ID = @BID";

                if (excludeId.HasValue)
                    query += " AND UOM_ID <> @ID";

                using var cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@DESC", description);
                cmd.Parameters.AddWithValue("@CODE", codeSunat);
                cmd.Parameters.AddWithValue("@BID", businessId);
                if (excludeId.HasValue) cmd.Parameters.AddWithValue("@ID", excludeId.Value);

                await connection.OpenAsync();
                var count = (int)await cmd.ExecuteScalarAsync();
                return count > 0;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al validar existencia de la unidad de medida.", ex.Message);
            }
        }

        public async Task<string> GetLastUomCodeAsync(long businessId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                string query = "SELECT MAX(SYMBOL) FROM UOM WHERE BUSINESS_ID = @BID";
                using var cmd = new SqlCommand(query, cn);
                cmd.Parameters.AddWithValue("@BID", businessId);

                object result = await cmd.ExecuteScalarAsync();
                return result == DBNull.Value || result == null ? null : result.ToString();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener el último código de unidad de medida.", ex.Message);
            }
        }

        public async Task AddAsync(Uom entity)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                using var cmd = new SqlCommand("SP_WS_REGISTER_UOM", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                cmd.Parameters.AddWithValue("@CODE_SUNAT", entity.CodeSunat);
                cmd.Parameters.AddWithValue("@SYMBOL", entity.Symbol);
                cmd.Parameters.AddWithValue("@DESCRIPTION", entity.Description);
                cmd.Parameters.AddWithValue("@CREATE_USER", entity.UsersBy);

                await connection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al registrar la unidad de medida en base de datos.", ex.Message);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al guardar la unidad de medida.", ex.Message);
            }
        }

        public async Task<PagedResult<Uom>> GetAllAsync(long businessId, string? search, int page, int pageSize, long? usersBy)
        {
            try
            {
                var list = new List<Uom>();
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_LIST_UOM", cn)
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
                    list.Add(new Uom
                    {
                        BusinessId = reader.GetInt64(0),
                        UomId = reader.GetInt64(1),
                        CodeSunat = reader.GetString(2),
                        Symbol = reader.GetString(3),
                        Description = reader.GetString(4),
                        Status = reader.GetString(5),
                        UomCount = reader.GetInt32(reader.GetOrdinal("UomCount"))
                    });
                }

                int total = 0;
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    total = reader.GetInt32(0);
                }

                return new PagedResult<Uom>
                {
                    Items = list,
                    Page = page,
                    PageSize = pageSize,
                    Total = total
                };
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener lista de unidades de medida.", ex.Message);
            }
        }

        public async Task<PagedSelect<OptionItem>> GetUomForSelectAsync(long businessId, string? search, int page, int pageSize)
        {
            var items = new List<OptionItem>();
            using var cn = _connectionFactory.CreateConnection();
            await cn.OpenAsync();

            using var cmd = new SqlCommand("SP_WS_UOM_SELECT", cn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 15 };
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

        public async Task<Uom> GetByIdAsync(long uomId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_UOM_BY_ID", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@UOM_ID", uomId);

                using var reader = await cmd.ExecuteReaderAsync();
                if (reader.HasRows && await reader.ReadAsync())
                {
                    return new Uom
                    {
                        UomId = reader.GetInt64(0),
                        BusinessId = reader.GetInt64(1),
                        CodeSunat = reader.GetString(2),
                        Symbol = reader.GetString(3),
                        Description = reader.GetString(4),
                    };
                }

                return null;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener la unidad de medida por ID.", ex.Message);
            }
        }

        public async Task<bool> UpdateAsync(Uom uom)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_UPDATE_UOM", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@UOM_ID", uom.UomId);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", uom.BusinessId);
                cmd.Parameters.AddWithValue("@CODE_SUNAT", uom.CodeSunat);
                cmd.Parameters.AddWithValue("@SYMBOL", uom.Symbol);
                cmd.Parameters.AddWithValue("@DESCRIPTION", uom.Description);
                cmd.Parameters.AddWithValue("@UPDATE_USER", uom.UsersBy);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar la unidad de medida.", ex.Message);
            }
        }

        public async Task<bool> PatchStatusAsync(long uomId, string status, int usersBy, long businessId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_UPDATE_UOM_STATUS", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
                cmd.Parameters.AddWithValue("@UOM_ID", uomId);
                cmd.Parameters.AddWithValue("@UPDATE_USER", usersBy);
                cmd.Parameters.AddWithValue("@STATUS", status);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al cambiar el estado de la unidad de medida.", ex.Message);
            }
        }
    }
}
