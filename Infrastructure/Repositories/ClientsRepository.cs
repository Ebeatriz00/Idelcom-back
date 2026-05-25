using Core.Entities;
using Core.Entities.paginations;
using Core.Interfaces;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class ClientsRepository : IClientsRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;

        public ClientsRepository(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<(bool exists, bool sameSeller, long? dbWorkerId, string? vendedor, string? clienteDb, string? lastActivityDesc, DateTime? lastActivityAt)> ExistsAsync(string documents, long businessId, long? workerId, long? excludeId = null)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_CLIENT_EXIST_WITH_LAST_ACTIVITY", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@DOCUMENTS", documents);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
                cmd.Parameters.AddWithValue("@EXCLUDE_ID", (object?)excludeId ?? DBNull.Value);

                using var reader = await cmd.ExecuteReaderAsync();

                if (!await reader.ReadAsync())
                    return (false, false, null, null, null, null, null);

                long? dbWorkerId = reader.IsDBNull(0) ? null : reader.GetInt64(0);
                string? vendedor = reader.IsDBNull(1) ? null : reader.GetString(1);
                string? clienteDb = reader.IsDBNull(2) ? null : reader.GetString(2);
                string? lastDesc = reader.IsDBNull(3) ? null : reader.GetString(3);
                DateTime? lastAt = reader.IsDBNull(4) ? null : reader.GetDateTime(4);

                bool sameSeller = dbWorkerId == workerId;

                return (true, sameSeller, dbWorkerId, vendedor, clienteDb, lastDesc, lastAt);
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al validar existencia del cliente.", ex.Message);
            }
        }

        public async Task<bool> ExistsContacts(long clientId)
        {
            const string sql = "SELECT CASE WHEN EXISTS (SELECT 1 FROM CONTACTS_CRM WHERE CLIENTS_ID = @CLIENTS_ID) THEN 1 ELSE 0 END";

            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                await using var command = new SqlCommand(sql, connection);

                command.Parameters.Add("@CLIENTS_ID", SqlDbType.BigInt).Value = clientId;

                var result = await command.ExecuteScalarAsync();
                return Convert.ToBoolean(result);
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al validar si el cliente tiene contactos.", ex.Message);
            }
        }


        public async Task<GlobalResponse> AddAsync(Clients entity)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                using var cmd = new SqlCommand("SP_WS_REGISTER_CLIENTS", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                cmd.Parameters.AddWithValue("@DOCUMENT_TYPE_ID", entity.DocumentTypeId);
                cmd.Parameters.AddWithValue("@DOCUMENTS", entity.Documents);
                cmd.Parameters.AddWithValue("@CLIENTS_NAME", entity.ClientsName);
                cmd.Parameters.AddWithValue("@CLIENTS_COMPANY", entity.ClientsCompany);
                cmd.Parameters.AddWithValue("@CLIENTS_ADDRESS", entity.ClientsAddress);
                cmd.Parameters.AddWithValue("@CLIENTS_PHONE", entity.ClientsPhone);
                cmd.Parameters.AddWithValue("@WORKER_ID", entity.WorkerId);
                cmd.Parameters.AddWithValue("@DEPARTAMENT_ID", entity.DepartmentId);
                cmd.Parameters.AddWithValue("@PROVINCE_ID", entity.ProvinceId);
                cmd.Parameters.AddWithValue("@DISTRICT_ID", entity.DistrictId);
                cmd.Parameters.AddWithValue("@PROCESS_TYPE_ID", entity.ProcessTypeId);
                cmd.Parameters.AddWithValue("@SECTOR_ID", entity.SectorId);
                cmd.Parameters.AddWithValue("@LEADS_SOURCES_ID", entity.LeadSourceId);
                cmd.Parameters.AddWithValue("@LEADS_STATUS_ID", entity.LeadStatusId);
                cmd.Parameters.AddWithValue("@LEADS_QUALIFICATIONS_ID", entity.LeadQualificationId);
                cmd.Parameters.AddWithValue("@WEB", entity.Website);
                cmd.Parameters.AddWithValue("@CREATE_USER", entity.UsersBy);

                await connection.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    var status = reader["status"] is int s
                        ? s
                        : Convert.ToInt32(reader["status"]);

                    var message = reader["message"]?.ToString() ?? string.Empty;

                    long? clientsId = null;
                    if (reader["CID"] != DBNull.Value)
                        clientsId = Convert.ToInt64(reader["CID"]);

                    return new GlobalResponse
                    {
                        Status = status,
                        Message = message,
                        Id = clientsId
                    };
                }

                throw new DatabaseException(
                    "Error al registrar al cliente en base de datos.",
                    "El procedimiento no devolvió resultados."
                );
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al registrar al cliente en base de datos.", ex.Message);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al guardar al cliente.", ex.Message);
            }
        }

        public async Task<PagedResult<Clients>> GetAllAsync(long businessId, string? search, int page, int pageSize, long? usersBy, bool? includeOthers)
        {
            try
            {
                var list = new List<Clients>();
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_LIST_CLIENTS", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
                cmd.Parameters.AddWithValue("@SEARCH", (object?)(string.IsNullOrWhiteSpace(search) ? null : search.Trim()) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PAGE", page);
                cmd.Parameters.AddWithValue("@PAGE_SIZE", pageSize);
                cmd.Parameters.AddWithValue("@WORKER_ID", (object?)usersBy ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@INCLUDE_OTHERS_IN_SEARCH", includeOthers);


                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    list.Add(new Clients
                    {
                        ClientsId = reader.GetInt64(0),
                        BusinessId = reader.GetInt64(1),
                        ClientsName = reader.GetString(2),
                        Documents = reader.GetString(3),
                        Sales = reader.IsDBNull(4) ? null : reader.GetString(4),
                        Sector = reader.IsDBNull(5) ? null : reader.GetString(5),
                        Departament = reader.IsDBNull(6) ? null : reader.GetString(6),
                        LeadStatus = reader.IsDBNull(7) ? null : reader.GetString(7),
                        Status = reader.GetString(8),
                        IsOtherSeller = reader.GetBoolean(9)
                    });
                }

                int total = 0;
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    total = reader.GetInt32(0);
                }

                return new PagedResult<Clients>
                {
                    Items = list,
                    Page = page,
                    PageSize = pageSize,
                    Total = total
                };
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener lista de cliente paginada.", ex.Message);
            }
        }

        public async Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, string? search, int page, int pageSize, long? usersBy)
        {
            try
            {
                var items = new List<OptionItem>();
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_CLIENTS_SELECT", cn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 15 };
                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
                cmd.Parameters.AddWithValue("@SEARCH", (object?)(string.IsNullOrWhiteSpace(search) ? null : search.Trim()) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PAGE", page);
                cmd.Parameters.AddWithValue("@PAGE_SIZE", pageSize);
                cmd.Parameters.AddWithValue("@WORKER_ID", (object?)usersBy ?? DBNull.Value);

                using var dr = await cmd.ExecuteReaderAsync();
                while (await dr.ReadAsync())
                {
                    items.Add(new OptionItem
                    {
                        Value = dr.GetInt64(0),
                        Label = dr.GetString(1),
                        ExtraInfo = dr.IsDBNull(2) ? null : dr.GetString(2)
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
                throw new DatabaseException("Error al obtener los cliente para el selector.", ex.Message);
            }
        }

        public async Task<List<Clients>> GetHistoryAsync(long? clientsId)
        {
            try
            {
                var list = new List<Clients>();
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_CHANGE_HISTORY_CLIENTS", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@CLIENTS_ID", clientsId);

                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    list.Add(new Clients
                    {
                        EventId = reader.GetInt64(0),
                        ChangeAt = reader.GetDateTime(1),
                        ChangeUser = reader.GetString(2),
                        Description = reader.GetString(3)
                    });
                }

                return list;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener historial del cliente.", ex.Message);
            }
        }


        public async Task<Clients> GetByIdAsync(long clientsId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();
                using var cmd = new SqlCommand("SP_WS_CLIENTS_BY_ID", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@CLIENTS_ID", clientsId);
                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new Clients
                    {
                        ClientsId = reader.GetInt64(0),
                        BusinessId = reader.GetInt64(1),
                        DocumentTypeId = reader.GetInt64(2),
                        Documents = reader.GetString(3),
                        ClientsName = reader.GetString(4),
                        ClientsCompany = reader.IsDBNull(5) ? null : reader.GetString(5),
                        ClientsAddress = reader.GetString(6),
                        ClientsPhone = reader.IsDBNull(7) ? null : reader.GetString(7),
                        WorkerId = reader.IsDBNull(8) ? null : reader.GetInt64(8),
                        DepartmentId = reader.IsDBNull(9) ? null : reader.GetInt64(9),
                        ProvinceId = reader.IsDBNull(10) ? null : reader.GetInt64(10),
                        DistrictId = reader.IsDBNull(11) ? null : reader.GetInt64(11),
                        ProcessTypeId = reader.IsDBNull(12) ? null : reader.GetInt64(12),
                        SectorId = reader.IsDBNull(13) ? null : reader.GetInt64(13),
                        LeadSourceId = reader.IsDBNull(14) ? null : reader.GetInt64(14),
                        LeadStatusId = reader.IsDBNull(15) ? null : reader.GetInt64(15),
                        LeadQualificationId = reader.IsDBNull(16) ? null : reader.GetInt64(16),
                        Website = reader.IsDBNull(17) ? null : reader.GetString(17)
                    };
                }
                return null;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener al cliente por ID.", ex.Message);
            }
        }

        public async Task<bool> UpdateAsync(Clients entity)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_UPDATE_CLIENTS", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@CLIENTS_ID", entity.ClientsId);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                cmd.Parameters.AddWithValue("@DOCUMENT_TYPE_ID", entity.DocumentTypeId);
                cmd.Parameters.AddWithValue("@DOCUMENTS", entity.Documents);
                cmd.Parameters.AddWithValue("@CLIENTS_NAME", entity.ClientsName);
                cmd.Parameters.AddWithValue("@CLIENT_COMPANY", entity.ClientsCompany);
                cmd.Parameters.AddWithValue("@CLIENT_ADDRESS", entity.ClientsAddress);
                cmd.Parameters.AddWithValue("@CLIENT_PHONE", entity.ClientsPhone);
                cmd.Parameters.AddWithValue("@WORKER_ID", entity.WorkerId);
                cmd.Parameters.AddWithValue("@DEPARTAMENT_ID", entity.DepartmentId);
                cmd.Parameters.AddWithValue("@PROVINCE_ID", entity.ProvinceId);
                cmd.Parameters.AddWithValue("@DISTRICT_ID", entity.DistrictId);
                cmd.Parameters.AddWithValue("@PROCESS_TYPE_ID", entity.ProcessTypeId);
                cmd.Parameters.AddWithValue("@SECTOR_ID", entity.SectorId);
                cmd.Parameters.AddWithValue("@LEADS_SOURCES_ID", entity.LeadSourceId);
                cmd.Parameters.AddWithValue("@LEADS_STATUS_ID", entity.LeadStatusId);
                cmd.Parameters.AddWithValue("@LEADS_QUALIFICATIONS_ID", entity.LeadQualificationId);
                cmd.Parameters.AddWithValue("@WEB", entity.Website);
                cmd.Parameters.AddWithValue("@UPDATE_USER", entity.UsersBy);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar al cliente en base de datos.", ex.Message);
            }
        }

        public async Task<bool> UpdateChangeSalesAsync(Clients entity)
        {
            try
            {


                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_UPDATE_CHANGE_SALES_CLIENTS", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@CLIENTS_ID", entity.ClientsId);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                cmd.Parameters.AddWithValue("@WORKER_ID", entity.WorkerId);
                cmd.Parameters.AddWithValue("@UPDATE_USER", entity.UsersBy);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar el estado del cliente.", ex.Message);
            }
        }

        public async Task<bool> PatchStatusAsync(long clientsId, string status, long userBy, long businessId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();
                using var cmd = new SqlCommand("SP_WS_UPDATE_CLIENTS_ACTIVE", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@CLIENTS_ID", clientsId);
                cmd.Parameters.AddWithValue("@STATUS", status);
                cmd.Parameters.AddWithValue("@UPDATE_USER", userBy);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar el estado del cliente.", ex.Message);
            }
        }

        public async Task<ClientsDetailDto> GetClientsDetailAsync(long clientsId, long businessId)
        {
            try
            {
                var result = new ClientsDetailDto();

                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_GET_CLIENTS_DETAIL", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@CLIENTS_ID", clientsId);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);

                using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    result.Header.ClientsId = reader.GetInt64(reader.GetOrdinal("CLIENTS_ID"));
                    result.Header.ClientsName = reader["CLIENTS_NAME"].ToString() ?? string.Empty;
                    result.Header.ClientAddress = reader["CLIENT_ADDRESS"] != DBNull.Value ? reader["CLIENT_ADDRESS"].ToString() : string.Empty;
                    result.Header.DepartamentName = reader["DEPARTAMENT_NAME"] != DBNull.Value ? reader["DEPARTAMENT_NAME"].ToString() : string.Empty;

                    result.Header.OpenOppCount = reader["OPEN_OPP_COUNT"] != DBNull.Value ? Convert.ToInt32(reader["OPEN_OPP_COUNT"]) : 0;
                    result.Header.OpenOppAmount = reader["OPEN_OPP_AMOUNT"] != DBNull.Value ? Convert.ToDecimal(reader["OPEN_OPP_AMOUNT"]) : 0;
                    result.Header.LtvTotalAmount = reader["LTV_TOTAL_AMOUNT"] != DBNull.Value ? Convert.ToDecimal(reader["LTV_TOTAL_AMOUNT"]) : 0;

                    result.Header.LastActivityAt = reader["LastActivityAt"] != DBNull.Value
                        ? (DateTime?)reader.GetDateTime(reader.GetOrdinal("LastActivityAt"))
                        : null;

                    result.Header.TotalQuotes = reader["TOTAL_QUOTES"] != DBNull.Value ? Convert.ToInt32(reader["TOTAL_QUOTES"]) : 0;
                    result.Header.WinRate = reader["WIN_RATE"] != DBNull.Value ? Convert.ToDecimal(reader["WIN_RATE"]) : 0;
                }
                else
                {
                    return result;
                }

                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
       
                        result.Pipeline.Add(new ClientsDetailOpportunityDto
                        {
                            OpporId = reader.GetString(reader.GetOrdinal("OPPOR_ID")),
                            OpporDesc = reader["OPPOR_DESC"].ToString() ?? string.Empty,
                            StateDesc = reader["STATE_DESC"] != DBNull.Value ? reader["STATE_DESC"].ToString() : "Sin Estado",
                            StateColor = reader["STATE_COLOR"] != DBNull.Value ? reader["STATE_COLOR"].ToString() : "#000000",
                            FinishDate = reader["FINISH_DATE"] != DBNull.Value ? (DateTime?)reader.GetDateTime(reader.GetOrdinal("FINISH_DATE")) : null,
                            Status = reader["STATUS"].ToString(),
                            OpportunityAmount = reader["OPPORTUNITY_AMOUNT"] != DBNull.Value ? Convert.ToDecimal(reader["OPPORTUNITY_AMOUNT"]) : 0
                        });
                    }
                }

                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        result.Contacts.Add(new ClientsDetailContactDto
                        {
                            ContactsCrmId = reader.GetInt64(reader.GetOrdinal("CONTACTS_CRM_ID")),
                            ContactName = reader["CONTACT_NAME"].ToString() ?? string.Empty,
                            JobTitle = reader["JOB_TITLE"] != DBNull.Value ? reader["JOB_TITLE"].ToString() : string.Empty
                        });
                    }
                }

                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        result.ActivityTrend.Add(new ClientActivityTrendDto
                        {
                            MonthName = reader["MonthName"].ToString() ?? string.Empty,
                            Year = Convert.ToInt32(reader["Year"]),
                            MonthNum = Convert.ToInt32(reader["MonthNum"]),
                            ActivityCount = reader["ActivityCount"] != DBNull.Value
                                ? Convert.ToInt32(reader["ActivityCount"])
                                : 0
                        });
                    }
                }

                return result;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener detalle del cliente.", ex.Message);
            }

        }
    }
}
