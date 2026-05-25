using Core.Entities;
using Core.Entities.paginations;
using Core.Interfaces;
using Core.Interfaces.Services;
using DocumentFormat.OpenXml.Spreadsheet;
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
    public class HiringRepository : IHiringRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;
        private readonly ILinkTokenService _linkTokenService;

        public HiringRepository(ISqlConnectionFactory connectionFactory, ILinkTokenService linkTokenService)
        {
            _connectionFactory = connectionFactory;
            _linkTokenService = linkTokenService; 
        }
        public async Task<GlobalResponse> AddAsync(Hiring entity)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                using var cmd = new SqlCommand("SP_WS_REGISTER_HIRING", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                cmd.Parameters.AddWithValue("@OPPOR_ID", entity.OpporId);
                cmd.Parameters.AddWithValue("@CREATE_USER", entity.UsersBy);

                await connection.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    var status = reader["status"] is int s
                        ? s
                        : Convert.ToInt32(reader["status"]);

                    var message = reader["message"]?.ToString() ?? string.Empty;

                    long? hiringsId = null;
                    if (reader["CID"] != DBNull.Value)
                        hiringsId = Convert.ToInt64(reader["CID"]);


                    return new GlobalResponse
                    {
                        Status = status,
                        Message = message,
                        Id = hiringsId
                    };
                }

                throw new DatabaseException(
                    "Error al registrar la solicitud en base de datos.",
                    "El procedimiento no devolvió resultados."
                );
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al registrar la solicitud en base de datos.", ex.Message);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al guardar la Solicitud.", ex.Message);
            }
        }


        public async Task<PagedResult<Hiring>> GetAllAsync(long businessId, string? search, int page, int pageSize, long? workerId, long? usersId)
        {
            try
            {
                var list = new List<Hiring>();
                int total = 0;

                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_LIST_OPPOR_HIRINGS", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 15;

                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
                cmd.Parameters.AddWithValue("@SEARCH", (object?)(string.IsNullOrWhiteSpace(search) ? null : search.Trim()) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PAGE", page);
                cmd.Parameters.AddWithValue("@PAGE_SIZE", pageSize);
                cmd.Parameters.AddWithValue("@WORKER_ID", (object?)workerId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@USERS_ID", (object?)usersId ?? DBNull.Value);

                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var opporId = reader.GetInt64(1);
                    long? tasksId = reader.IsDBNull(14) ? (long?)null : reader.GetInt64(14);
                    list.Add(new Hiring
                    {
                        HiringId = reader.GetInt64(0),
                        OpporId = opporId, 
                        LinkToken = _linkTokenService.Issue("opportunity", Convert.ToInt64(opporId), TimeSpan.FromHours(1)),
                        BusinessId = reader.GetInt64(2),
                        OpporNum = reader.GetString(3),
                        OpporDesc = reader.GetString(4),
                        ClientsName = reader.IsDBNull(5) ? null : reader.GetString(5),
                        HiringStatus = reader.IsDBNull(6) ? null : reader.GetString(6),
                        HiringStatusColor = reader.IsDBNull(7) ? null : reader.GetString(7),
                        OpporStatus = reader.IsDBNull(8) ? null : reader.GetString(8),
                        OpporStatusColor = reader.IsDBNull(9) ? null : reader.GetString(9),
                        ProcessType = reader.IsDBNull(10) ? null : reader.GetInt32(10),
                        LicStatusId = reader.IsDBNull(11) ? null : reader.GetInt64(11),
                        UnreadFilesCount = reader.IsDBNull(12) ? null : reader.GetInt32(12),
                        StateOpportunityId = reader.IsDBNull(13) ? (long?)null : reader.GetInt64(13),
                        TasksId = tasksId,
                        TaskToken = _linkTokenService.Issue("tasks", Convert.ToInt64(tasksId), TimeSpan.FromHours(1)),
                        StateTaskId = reader.IsDBNull(15) ? (long?)null : reader.GetInt64(15),
                        TaskDesc = reader.IsDBNull(16) ? null : reader.GetString(16),
                        TaskColor = reader.IsDBNull(17) ? null : reader.GetString(17),
                        TypeDeliverable = reader.IsDBNull(18) ? null : reader.GetInt32(18),
                        RequestNote = reader.IsDBNull(19) ? null : reader.GetString(19),
                        ObsStatusId = reader.IsDBNull(20) ? null : reader.GetInt64(20),
                        StatePreSalesId = reader.IsDBNull(21) ? null : reader.GetInt64(21),
                        TypeObsClients = reader.IsDBNull(22) ? null : reader.GetInt32(22),
                        TypeObsEconomic = reader.IsDBNull(23) ? null : reader.GetInt32(23),
                        AffectsQuotatation = reader.IsDBNull(24) ? null : reader.GetBoolean(24),

                        HiringFiles = new List<HiringFile>()
                    });
                }

                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    total = reader.GetInt32(0);
                }

                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var fOpporId = reader.GetInt64(0); 

                        var parent = list.FirstOrDefault(x => x.OpporId == fOpporId);

                        if (parent != null)
                        {
                            parent.HiringFiles.Add(new HiringFile
                            {
                                FileTitle = reader.GetString(1),
                                FileUrl = reader.GetString(2),
                                RelativePath = reader.GetString(3),
                                ArchiveType = reader.GetString(4)
                            });
                        }
                    }
                }

                return new PagedResult<Hiring>
                {
                    Items = list,
                    Page = page,
                    PageSize = pageSize,
                    Total = total
                };
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener lista paginada.", ex.Message);
            }
        }

        public async Task<GlobalResponse> UpdateStatusAsync(Hiring entity)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_UPDATE_HIRING_STATUS", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@HIRING_ID", entity.HiringId);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                cmd.Parameters.AddWithValue("@LIC_STATUS_ID", entity.LicStatusId);
                cmd.Parameters.AddWithValue("@UPDATE_USER", entity.UsersBy);

                var tvpFile = BuildDeliverablesHiringFileTvp(entity.HiringFiles ?? new List<HiringFile>());

                var pFiles = cmd.Parameters.Add("@HIRING_FILES", SqlDbType.Structured);
                pFiles.TypeName = "dbo.TT_FILE_TRACKING"; 
                pFiles.Value = tvpFile;

                using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new GlobalResponse
                    {
                        Status = Convert.ToInt32(reader["status"]),
                        Message = reader["message"]?.ToString() ?? "Proceso completado."
                    };
                }

                return new GlobalResponse { Status = 0, Message = "El servidor no devolvió respuesta." };
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error de base de datos al actualizar contratación.", ex.Message);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al actualizar contratación.", ex.Message);
            }
        }


        private DataTable BuildDeliverablesHiringFileTvp(List<HiringFile> files)
        {
            var table = new DataTable();


            table.Columns.Add("FILE_TRACKING_ID", typeof(int));      
            table.Columns.Add("FILE_TITLE", typeof(string));        
            table.Columns.Add("FILE_URL", typeof(string));         
            table.Columns.Add("RELATIVE_PATH", typeof(string));
            table.Columns.Add("ARCHIVE_TYPE", typeof(string));


            if (files != null)
            {
                foreach (var file in files)
                {
                    if (string.IsNullOrEmpty(file.FileUrl)) continue;


                    table.Rows.Add(
                        0,                          
                        file.FileTitle ?? "",          
                        file.FileUrl ?? "",            
                        file.RelativePath ?? "",
                        file.ArchiveType
                    );
                }
            }

            return table;
        }

        public async Task MarkFilesReadAsync(FileTracking model)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();
                using var cmd = new SqlCommand("SP_WS_MARK_OPPOR_FILES_READ", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@BUSINESS_ID", model.BusinessId);
                cmd.Parameters.AddWithValue("@OPPOR_ID", model.OpporToken);
                cmd.Parameters.AddWithValue("@USERS_ID", model.UsersBy);

                await cmd.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException(
                    "Error al marcar los archivos como leídos en base de datos.",
                    ex.Message
                );
            }
            catch (Exception ex)
            {
                throw new DatabaseException(
                    "Error inesperado al marcar los archivos como leídos.",
                    ex.Message
                );
            }
        }




    }
}
