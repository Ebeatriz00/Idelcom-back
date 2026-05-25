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
    public class ConceptTypeRepository : IConceptTypeRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;

        public ConceptTypeRepository(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<bool> ExistsAsync(string description, long businessId, long? excludeId = null)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                
                string query = "SELECT COUNT(*) FROM CONCEPT_TYPE WHERE DESCRIPTION = @DESC AND BUSINESS_ID = @BID";

                if (excludeId.HasValue)
                    query += " AND CONCEPT_TYPE_ID <> @ID"; 

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
              
                throw new DatabaseException("Error al validar existencia del tipo de concepto.", ex.Message);
            }
        }

    
        public async Task AddAsync(ConceptType entity)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                using var cmd = new SqlCommand("SP_WS_REGISTER_CONCEPT_TYPE", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                cmd.Parameters.AddWithValue("@DESCRIPTION", entity.Description); 
                cmd.Parameters.AddWithValue("@CREATE_USER", entity.UsersBy); 
                                                                             

                await connection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al registrar el tipo de concepto en base de datos.", ex.Message);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al guardar el tipo de concepto.", ex.Message);
            }
        }

        public async Task<PagedResult<ConceptType>> GetAllAsync(long businessId, string? search, int page, int pageSize)
        {
            try
            {
                var list = new List<ConceptType>();
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_LIST_CONCEPT_TYPE", cn)
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
                    list.Add(new ConceptType 
                    {
                        ConceptTypeId = reader.GetInt64(0), 
                        Description = reader.GetString(1),  
                        Status = reader.GetString(2),       
                        ConceptTypeCount = reader.GetInt32(reader.GetOrdinal("ConceptTypeCount")) // Asume que el SP devuelve este campo
                    });
                }
                int total = 0;
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    total = reader.GetInt32(0);
                }

                return new PagedResult<ConceptType>
                {
                    Items = list,
                    Page = page,
                    PageSize = pageSize,
                    Total = total
                };
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener lista de tipos de conceptos.", ex.Message);
            }
        }

        public async Task<PagedSelect<OptionItem>> GetConceptTypesForSelectAsync(long businessId, string? search, int page, int pageSize)
        {
            var items = new List<OptionItem>();
            using var cn = _connectionFactory.CreateConnection();
            await cn.OpenAsync();

            using var cmd = new SqlCommand("SP_WS_CONCEPT_TYPE_SELECT", cn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 15 };
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


        public async Task<ConceptType> GetByIdAsync(long conceptTypeId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_CONCEPT_TYPE_BY_ID", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@CONCEPT_TYPE_ID", conceptTypeId);

                using var reader = await cmd.ExecuteReaderAsync();
                if (reader.HasRows && await reader.ReadAsync())
                {
                    return new ConceptType
                    {
                        ConceptTypeId = reader.GetInt64(0),
                        BusinessId = reader.GetInt64(1),
                        Description = reader.GetString(2),
                       
                    };
                }

                return null;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener el tipo de concepto por ID.", ex.Message);
            }
        }

        public async Task<bool> UpdateAsync(ConceptType entity)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_UPDATE_CONCEPT_TYPE", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@CONCEPT_TYPE_ID", entity.ConceptTypeId);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                cmd.Parameters.AddWithValue("@DESCRIPTION", entity.Description);
                cmd.Parameters.AddWithValue("@UPDATE_USER", entity.UsersBy); 
                                                                          

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar el tipo de concepto.", ex.Message);
            }
        }

        public async Task<bool> PatchStatusAsync(long conceptTypeId, string status, int UsersBy, long businessId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_UPDATE_CONCEPT_TYPE_STATUS", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
                cmd.Parameters.AddWithValue("@CONCEPT_TYPE_ID", conceptTypeId);
                cmd.Parameters.AddWithValue("@UPDATE_USER", UsersBy);
                cmd.Parameters.AddWithValue("@STATUS", status);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al cambiar el estado del tipo de concepto.", ex.Message);
            }
        }
    }
}
