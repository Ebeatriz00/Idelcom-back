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
    public class MovementTypesRepository : IMovementTypesRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;

        public MovementTypesRepository(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        private async Task<bool> ExistsAsync(string field, string value, long businessId, long? excludeId = null)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var query = new StringBuilder($"SELECT COUNT(*) FROM dbo.MOVEMENT_TYPES WHERE {field} = @VALUE AND BUSINESS_ID = @BID");

                if (excludeId.HasValue)
                {
                    query.Append(" AND MOVEMENT_TYPES_ID <> @ID");
                }

                using var cmd = new SqlCommand(query.ToString(), connection);
                cmd.Parameters.AddWithValue("@VALUE", value);
                cmd.Parameters.AddWithValue("@BID", businessId);
                if (excludeId.HasValue)
                {
                    cmd.Parameters.AddWithValue("@ID", excludeId.Value);
                }

                await connection.OpenAsync();
                var count = (int)await cmd.ExecuteScalarAsync();
                return count > 0;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException($"Error al validar existencia del tipo de movimiento por {field}.", ex.Message);
            }
        }

        public Task<bool> ExistsByDescriptionAsync(string description, long businessId, long? excludeId = null)
        {
            return ExistsAsync("DESCRIPTION", description, businessId, excludeId);
        }

        public Task<bool> ExistsByCodeAsync(string code, long businessId, long? excludeId = null)
        {
            return ExistsAsync("CODE", code, businessId, excludeId);
        }

        public async Task AddAsync(MovementTypes entity)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                using var cmd = new SqlCommand("SP_WS_REGISTER_MOVEMENT_TYPES", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                cmd.Parameters.AddWithValue("@CODE", entity.Code);
                cmd.Parameters.AddWithValue("@DESCRIPTION", entity.Description);
                cmd.Parameters.AddWithValue("@MOV_SUNAT_ID", entity.MovSunatId);
                cmd.Parameters.AddWithValue("@MOV_OPER_ID", entity.MovOperId);
                cmd.Parameters.AddWithValue("@MOV_PER_ID", entity.MovPerId);
                cmd.Parameters.AddWithValue("@MOV_CLAS_ID", entity.MovClasId);
                cmd.Parameters.AddWithValue("@AFFECTS_STOCK", entity.AffectsStock);
                cmd.Parameters.AddWithValue("@REQUIRES_DEST_WARE", entity.RequiresDestWare);
                cmd.Parameters.AddWithValue("@GENERATES_ACCOUNTING", entity.GeneratesAccounting);
                cmd.Parameters.AddWithValue("@IS_ADJUSTMENT", entity.IsAdjustment);
                cmd.Parameters.AddWithValue("@ALLOW_NEGATIVE", entity.AllowNegative);
                cmd.Parameters.AddWithValue("@CREATE_USER", entity.UsersBy);

                await connection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al registrar el tipo de movimiento en base de datos.", ex.Message);
            }
        }

        public async Task<PagedResult<MovementTypes>> GetAllAsync(long businessId, string? search, int page, int pageSize, long? usersBy)
        {
            try
            {
                var list = new List<MovementTypes>();
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_LIST_MOVEMENT_TYPES", cn)
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
                    list.Add(new MovementTypes
                    {
                        MovementTypesId = reader.GetInt64(0),
                        Code = reader.GetString(1),
                        Description = reader.GetString(2),
                        MovSunatDescription = reader.IsDBNull(3) ? null : reader.GetString(3),
                        MovOperDescription = reader.IsDBNull(4) ? null : reader.GetString(4),
                        MovPerDescription = reader.IsDBNull(5) ? null :  reader.GetString(5),
                        MovClasDescription = reader.IsDBNull(6) ? null :  reader.GetString(6),
                        Status = reader.GetString(7),
                        MovemenTypesCount = reader.GetInt32(8) 
                    });
                }

                int total = 0;
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    total = reader.GetInt32(0);
                }

                return new PagedResult<MovementTypes>
                {
                    Items = list,
                    Page = page,
                    PageSize = pageSize,
                    Total = total
                };
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener lista de tipos de movimiento paginada.", ex.Message);
            }
        }

        public async Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, string? search, int page, int pageSize)
        {
            try
            {
                var items = new List<OptionItem>();
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_MOVEMENT_TYPES_SELECT", cn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 15 };
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
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener los tipos de movimiento para el selector.", ex.Message);
            }
        }

        public async Task<MovementTypes> GetByIdAsync(long movementTypesId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();
                using var cmd = new SqlCommand("SP_WS_MOVEMENT_TYPES_BY_ID", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@MOVEMENT_TYPES_ID", movementTypesId);
                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new MovementTypes
                    {
                        MovementTypesId = reader.GetInt64(0),
                        BusinessId = reader.GetInt64(1),
                        Code = reader.GetString(2),
                        Description = reader.GetString(3),
                        MovSunatId = reader.GetInt64(4),
                        MovOperId = reader.GetInt64(5),
                        MovPerId = reader.GetInt64(6),
                        MovClasId = reader.GetInt64(7),
                        AffectsStock = reader.GetBoolean(8),
                        RequiresDestWare = reader.GetBoolean(9),
                        GeneratesAccounting = reader.GetBoolean(10),
                        IsAdjustment = reader.GetBoolean(11),
                        AllowNegative = reader.GetBoolean(12),
                    };
                }
                return null;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener el tipo de movimiento por ID.", ex.Message);
            }
        }

        public async Task<MovementTypes?> GetByCodeAsync(string code, long businessId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();
                const string query = """
                    SELECT TOP 1
                        MOVEMENT_TYPES_ID, BUSINESS_ID, CODE, DESCRIPTION,
                        MOV_SUNAT_ID, MOV_OPER_ID, MOV_PER_ID, MOV_CLAS_ID,
                        ISNULL(AFFECTS_STOCK, 0), ISNULL(REQUIRES_DEST_WARE, 0),
                        ISNULL(GENERATES_ACCOUNTING, 0), ISNULL(IS_ADJUSTMENT, 0), ISNULL(ALLOW_NEGATIVE, 0)
                    FROM dbo.MOVEMENT_TYPES
                    WHERE CODE = @CODE AND BUSINESS_ID = @BUSINESS_ID AND STATUS = 1
                    """;
                using var cmd = new SqlCommand(query, cn);
                cmd.Parameters.AddWithValue("@CODE", code);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new MovementTypes
                    {
                        MovementTypesId = reader.GetInt64(0),
                        BusinessId = reader.GetInt64(1),
                        Code = reader.GetString(2),
                        Description = reader.GetString(3),
                        MovSunatId = reader.GetInt64(4),
                        MovOperId = reader.GetInt64(5),
                        MovPerId = reader.GetInt64(6),
                        MovClasId = reader.GetInt64(7),
                        AffectsStock = reader.GetBoolean(8),
                        RequiresDestWare = reader.GetBoolean(9),
                        GeneratesAccounting = reader.GetBoolean(10),
                        IsAdjustment = reader.GetBoolean(11),
                        AllowNegative = reader.GetBoolean(12),
                    };
                }
                return null;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener el tipo de movimiento por codigo.", ex.Message);
            }
        }

        public async Task<bool> UpdateAsync(MovementTypes entity)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_UPDATE_MOVEMENT_TYPES", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@MOVEMENT_TYPES_ID", entity.MovementTypesId);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                cmd.Parameters.AddWithValue("@CODE", entity.Code);
                cmd.Parameters.AddWithValue("@DESCRIPTION", entity.Description);
                cmd.Parameters.AddWithValue("@MOV_SUNAT_ID", entity.MovSunatId);
                cmd.Parameters.AddWithValue("@MOV_OPER_ID", entity.MovOperId);
                cmd.Parameters.AddWithValue("@MOV_PER_ID", entity.MovPerId);
                cmd.Parameters.AddWithValue("@MOV_CLAS_ID", entity.MovClasId);
                cmd.Parameters.AddWithValue("@AFFECTS_STOCK", entity.AffectsStock);
                cmd.Parameters.AddWithValue("@REQUIRES_DEST_WARE", entity.RequiresDestWare);
                cmd.Parameters.AddWithValue("@GENERATES_ACCOUNTING", entity.GeneratesAccounting);
                cmd.Parameters.AddWithValue("@IS_ADJUSTMENT", entity.IsAdjustment);
                cmd.Parameters.AddWithValue("@ALLOW_NEGATIVE", entity.AllowNegative);
                cmd.Parameters.AddWithValue("@UPDATE_USER", entity.UsersBy);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar el tipo de movimiento en base de datos.", ex.Message);
            }
        }

        public async Task<bool> PatchStatusAsync(long movementTypesId, string status, long usersBy, long businessId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();
                using var cmd = new SqlCommand("SP_WS_UPDATE_MOVEMENT_TYPES_ACTIVE", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@MOVEMENT_TYPES_ID", movementTypesId);
                cmd.Parameters.AddWithValue("@STATUS", status);
                cmd.Parameters.AddWithValue("@UPDATE_USER", usersBy);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar el estado del tipo de movimiento.", ex.Message);
            }
        }
    }
}
