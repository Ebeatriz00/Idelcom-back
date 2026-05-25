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
    public class SsomaMovementTypeRepository : ISsomaMovementTypeRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;

        public SsomaMovementTypeRepository(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task AddAsync(SsomaMovementType entity)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                using var cmd = new SqlCommand("SP_WS_REGISTER_SSOMA_MOVEMENT_TYPE", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                cmd.Parameters.AddWithValue("@SSOMA_MOVEMENT_TYPE_DESC", entity.SsomaMovementTypeDesc);
                cmd.Parameters.AddWithValue("@CREATE_USER", entity.UsersBy);

                await connection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al registrar el tipo de movimiento SSOMA en base de datos.", ex.Message);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al guardar el tipo de movimiento SSOMA.", ex.Message);
            }
        }

        public async Task<PagedResult<SsomaMovementType>> GetAllAsync(long businessId, string? search, int page, int pageSize, long? usersBy)
        {
            try
            {
                var list = new List<SsomaMovementType>();
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_LIST_SSOMA_MOVEMENT_TYPES", cn)
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
                    list.Add(new SsomaMovementType
                    {
                        SsomaMovementTypeId = reader.GetInt32(0),
                        SsomaMovementTypeDesc = reader.GetString(1),
                        Status = reader.GetString(2),
                        BusinessId = reader.GetInt64(3)
                    });
                }

                int total = 0;
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    total = reader.GetInt32(0);
                }

                return new PagedResult<SsomaMovementType>
                {
                    Items = list,
                    Page = page,
                    PageSize = pageSize,
                    Total = total
                };
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener lista de tipos de movimiento SSOMA paginada.", ex.Message);
            }
        }

        public async Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, string? search, int page, int pageSize)
        {
            try
            {
                var items = new List<OptionItem>();
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_SSOMA_MOVEMENT_TYPE_SELECT", cn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 15 };
                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
                cmd.Parameters.AddWithValue("@SEARCH", (object?)(string.IsNullOrWhiteSpace(search) ? null : search.Trim()) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PAGE", page);
                cmd.Parameters.AddWithValue("@PAGE_SIZE", pageSize);

                using var dr = await cmd.ExecuteReaderAsync();
                while (await dr.ReadAsync())
                {
                    items.Add(new OptionItem
                    {
                        Value = dr.GetInt32(0),
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
                throw new DatabaseException("Error al obtener los tipos de movimiento SSOMA para el selector.", ex.Message);
            }
        }

        public async Task<SsomaMovementType> GetByIdAsync(long ssomaMovementTypeId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();
                using var cmd = new SqlCommand("SP_WS_SSOMA_MOVEMENT_TYPE_BY_ID", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@SSOMA_MOVEMENT_TYPE_ID", ssomaMovementTypeId);
                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new SsomaMovementType
                    {
                        SsomaMovementTypeId = reader.GetInt32(0),
                        BusinessId = reader.GetInt64(1),
                        SsomaMovementTypeDesc = reader.GetString(2)
                    };
                }
                return null!;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener el tipo de movimiento SSOMA por ID.", ex.Message);
            }
        }

        public async Task<bool> UpdateAsync(SsomaMovementType entity)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_UPDATE_SSOMA_MOVEMENT_TYPE", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@SSOMA_MOVEMENT_TYPE_ID", entity.SsomaMovementTypeId);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                cmd.Parameters.AddWithValue("@SSOMA_MOVEMENT_TYPE_DESC", entity.SsomaMovementTypeDesc);
                cmd.Parameters.AddWithValue("@UPDATE_USER", entity.UsersBy);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar el tipo de movimiento SSOMA en base de datos.", ex.Message);
            }
        }

        public async Task<bool> PatchStatusAsync(long ssomaMovementTypeId, string status, long usersBy, long businessId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();
                using var cmd = new SqlCommand("SP_WS_UPDATE_SSOMA_MOVEMENT_TYPE_ACTIVE", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@SSOMA_MOVEMENT_TYPE_ID", ssomaMovementTypeId);
                cmd.Parameters.AddWithValue("@STATUS", status);
                cmd.Parameters.AddWithValue("@UPDATE_USER", usersBy);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar el estado del tipo de movimiento SSOMA.", ex.Message);
            }
        }
    }
}
