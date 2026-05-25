using Core.Entities;
using Core.Entities.paginations;
using Core.Interfaces;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    


    public class ContactsCrmRepository : IContactsCrmRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;

        public ContactsCrmRepository(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<bool> ExistsAsync(string contactsName, long businessId, long? exclude = null)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var query = new StringBuilder(@"
                                    SELECT COUNT(*)
                                    FROM dbo.CONTACTS_CRM
                                    WHERE CONTACT_NAME LIKE '%' + @CONTACT_NAME + '%'
                                    AND BUSINESS_ID = @BID
                                    ");
                if (exclude.HasValue)
                {
                    query.Append(" AND CONTACTS_CRM_ID <> @ID");
                }

                using var cmd = new SqlCommand(query.ToString(), connection);
                cmd.Parameters.AddWithValue("@CONTACT_NAME", contactsName);
                cmd.Parameters.AddWithValue("@BID", businessId);
                if (exclude.HasValue)
                {
                    cmd.Parameters.AddWithValue("@ID", exclude.Value);
                }

                await connection.OpenAsync();
                var count = (int)await cmd.ExecuteScalarAsync();
                return count > 0;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al validar existencia del contacto.", ex.Message);
            }
        }

        public async Task AddAsync(ContactsCrm entity)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                using var cmd = new SqlCommand("SP_WS_REGISTER_CONTACTS_CRM", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                cmd.Parameters.AddWithValue("@CONTACT_NAME", entity.ContactName);
                cmd.Parameters.AddWithValue("@JOB_TITLE", entity.JobTitle);
                cmd.Parameters.AddWithValue("@PHONE", (object)entity.Phone ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MOVIL", (object)entity.Movil ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@EMAIL", entity.Email);
                cmd.Parameters.AddWithValue("@WORKER_ID", entity.WorkerId);
                cmd.Parameters.AddWithValue("@CLIENTS_ID", entity.ClientsId);
                cmd.Parameters.AddWithValue("@LEADS_SOURCES_ID", entity.LeadsSourcesId);
                cmd.Parameters.AddWithValue("@CONTACT_TYPE_ID", entity.ContactTypeId);
                cmd.Parameters.AddWithValue("@CREATE_USER", entity.UsersBy);

                await connection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al registrar el contacto en base de datos.", ex.Message);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al guardar el contacto.", ex.Message);
            }
        }

        public async Task<PagedResult<ContactsCrm>> GetAllAsync(long businessId, string? search, int page, int pageSize, long? usersBy)
        {
            try
            {
                var list = new List<ContactsCrm>();
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_LIST_CONTACTS_CRM", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
                cmd.Parameters.AddWithValue("@SEARCH", (object?)(string.IsNullOrWhiteSpace(search) ? null : search.Trim()) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PAGE", page);
                cmd.Parameters.AddWithValue("@PAGE_SIZE", pageSize);
                cmd.Parameters.AddWithValue("@WORKER_ID", (object?)usersBy ?? DBNull.Value);

                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    list.Add(new ContactsCrm
                    {
 
                        ContactsCrmId = reader.GetInt64(0),
                        BusinessId = reader.GetInt64(1),
                        ContactName =  reader.IsDBNull(2) ? null : reader.GetString(2),
                        Status = reader.GetString(3),
                        JobTitle = reader.IsDBNull(4) ? null : reader.GetString(4),
                        Movil = reader.IsDBNull(5) ? null : reader.GetString(5),
                        Email = reader.IsDBNull(6) ? null : reader.GetString(6),
                        WorkerDescritpion = reader.IsDBNull(7) ? null : reader.GetString(7),
                        ClientsDescription = reader.IsDBNull(8) ? null : reader.GetString(8),
                        LeadsSourcesDescription = reader.IsDBNull(9) ? null : reader.GetString(9),
                        ContactTypeDescription = reader.IsDBNull(10) ? null : reader.GetString(10),
                        ContactsCrmCount = reader.GetInt32(11)
                        
                    });
                }

                int total = 0;
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    total = reader.GetInt32(0);
                }

                return new PagedResult<ContactsCrm>
                {
                    Items = list,
                    Page = page,
                    PageSize = pageSize,
                    Total = total
                };
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener lista de contactos paginada.", ex.Message);
            }
        }

        public async Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, long clientsId, long? workerId, string? search, int page, int pageSize)
        {
            try
            {
                var items = new List<OptionItem>();
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_CONTACTS_CRM_SELECT", cn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 15 };
                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
                cmd.Parameters.AddWithValue("@CLIENTS_ID", clientsId);

                object workerParam = workerId > 0 ? workerId : DBNull.Value;
                cmd.Parameters.AddWithValue("@WORKER_ID", workerParam);

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
                throw new DatabaseException("Error al obtener los contactos para el selector.", ex.Message);
            }
        }


        public async Task<ContactsCrm> GetByIdAsync(long contactsCrmId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();
                using var cmd = new SqlCommand("SP_WS_CONTACTS_CRM_BY_ID", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@CONTACTS_CRM_ID", contactsCrmId);
                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {

                    return new ContactsCrm
                    {
                        ContactsCrmId = reader.GetInt64(0),
                        BusinessId = reader.GetInt64(1),
                        ContactName = reader.IsDBNull(2) ? null : reader.GetString(2),
                        JobTitle = reader.IsDBNull(3) ? null : reader.GetString(3),
                        Phone = reader.IsDBNull(4) ? null : reader.GetString(4),
                        Movil = reader.IsDBNull(5) ? null : reader.GetString(5),
                        Email = reader.IsDBNull(6) ? null : reader.GetString(6),
                        WorkerId = reader.IsDBNull (7) ? null : reader.GetInt64(7),
                        ClientsId = reader.IsDBNull(8) ? null : reader.GetInt64(8),
                        LeadsSourcesId = reader.IsDBNull(9) ? null : reader.GetInt64(9),
                        ContactTypeId = reader.IsDBNull(10) ? null : reader.GetInt64(10)

       
                    };
                }
                return null; 
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener el contacto por ID.", ex.Message);
            }
        }

        public async Task<bool> UpdateAsync(ContactsCrm entity)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();


                using var cmd = new SqlCommand("SP_WS_UPDATE_CONTACTS_CRM", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };


                cmd.Parameters.AddWithValue("@CONTACTS_CRM_ID", entity.ContactsCrmId);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                cmd.Parameters.AddWithValue("@CONTACT_NAME", entity.ContactName);
                cmd.Parameters.AddWithValue("@JOB_TITLE", entity.JobTitle);
                cmd.Parameters.AddWithValue("@PHONE", (object)entity.Phone ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MOVIL", (object)entity.Movil ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@EMAIL", entity.Email);
                cmd.Parameters.AddWithValue("@WORKER_ID", entity.WorkerId);
                cmd.Parameters.AddWithValue("@CLIENTS_ID", entity.ClientsId);
                cmd.Parameters.AddWithValue("@LEADS_SOURCES_ID", entity.LeadsSourcesId);
                cmd.Parameters.AddWithValue("@CONTACT_TYPE_ID", entity.ContactTypeId);
                cmd.Parameters.AddWithValue("@UPDATE_USER", entity.UsersBy);


                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar el contacto en base de datos.", ex.Message);
            }
        }

        public async Task<bool> PatchStatusAsync(long contactsCrmId, string status, long usersBy, long businessId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();
                using var cmd = new SqlCommand("SP_WS_UPDATE_CONTACTS_CRM_ACTIVE", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@CONTACTS_CRM_ID", contactsCrmId);
                cmd.Parameters.AddWithValue("@STATUS", status);
                cmd.Parameters.AddWithValue("@UPDATE_USER", usersBy);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar el estado del contacto.", ex.Message);
            }
        }
    }
}
 
