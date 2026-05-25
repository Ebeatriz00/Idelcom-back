using Core.Entities;
using Core.Interfaces;
using Core.Interfaces.Services;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class ObservationsRepository : IObservationsRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;
        private readonly ILinkTokenService _linkTokenService;

        public ObservationsRepository(ISqlConnectionFactory connectionFactory, ILinkTokenService linkTokenService)
        {
            _connectionFactory = connectionFactory;
            _linkTokenService = linkTokenService;
        }

        public async Task AddAsync(IEnumerable<Observations> entities)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var tx = connection.BeginTransaction();

            try
            {
                foreach (var entity in entities)
                {
                    using var cmd = new SqlCommand("SP_WS_REGISTER_OBSERVATIONS", connection, tx)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = 15
                    };

                    cmd.Parameters.AddWithValue("@BUSINESS_ID", (object?)entity.BusinessId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@OPPOR_ID", (object?)entity.OpporId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@OBS_TYPE", (object?)entity.ObsType ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@OBS_SEVERITY", (object?)entity.ObsSeverity ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@OBS_STATUS_ID", (object?)entity.ObsStatusId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@OBS_REASON", (object?)entity.ObsReason ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DUE_DATE", (object?)entity.DueDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DUE_SET_BY", (object?)entity.DueSetBy ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@OPENED_BY", (object?)entity.OpenedBy ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@OPENED_AT", (object?)entity.OpenedAt ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@STATUS", (object?)entity.Status ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@AFFECTS_QUOTATION", (object?)entity.AffectsQuotation ?? DBNull.Value);

                    await cmd.ExecuteNonQueryAsync();
                }

                await tx.CommitAsync();
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                throw new DatabaseException("Error al registrar múltiples observaciones.", ex.Message);
            }
        }

        public async Task<Observations> GetByIdAsync(long obsId)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_OBSERVATIONS_GET_BY_ID", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@OBS_ID", obsId);

                using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    var opporId = reader.GetInt64(reader.GetOrdinal("OPPOR_ID"));
                    return new Observations
                    {
                        ObsId = reader.GetInt64(reader.GetOrdinal("OBS_ID")),
                        BusinessId = reader.IsDBNull(reader.GetOrdinal("BUSINESS_ID")) ? null : reader.GetInt64(reader.GetOrdinal("BUSINESS_ID")),
                        OpporToken = _linkTokenService.Issue("opportunity", opporId, TimeSpan.FromHours(1)),
                        ObsType = reader.IsDBNull(reader.GetOrdinal("OBS_TYPE")) ? null : reader.GetInt32(reader.GetOrdinal("OBS_TYPE")),
                        ObsSeverity = reader.IsDBNull(reader.GetOrdinal("OBS_SEVERITY")) ? null : reader.GetInt32(reader.GetOrdinal("OBS_SEVERITY")),
                        ObsStatusId = reader.IsDBNull(reader.GetOrdinal("OBS_STATUS_ID")) ? null : reader.GetInt64(reader.GetOrdinal("OBS_STATUS_ID")),
                        ObsReason = reader.IsDBNull(reader.GetOrdinal("OBS_REASON")) ? null : reader.GetString(reader.GetOrdinal("OBS_REASON")),
                        DueDate = reader.IsDBNull(reader.GetOrdinal("DUE_DATE")) ? null : reader.GetDateTime(reader.GetOrdinal("DUE_DATE")),
                        DueSetBy = reader.IsDBNull(reader.GetOrdinal("DUE_SET_BY")) ? null : reader.GetInt64(reader.GetOrdinal("DUE_SET_BY")),
                        OpenedBy = reader.IsDBNull(reader.GetOrdinal("OPENED_BY")) ? null : reader.GetInt64(reader.GetOrdinal("OPENED_BY")),
                        OpenedAt = reader.IsDBNull(reader.GetOrdinal("OPENED_AT")) ? null : reader.GetDateTime(reader.GetOrdinal("OPENED_AT")),
                        ClosedAt = reader.IsDBNull(reader.GetOrdinal("CLOSED_AT")) ? null : reader.GetDateTime(reader.GetOrdinal("CLOSED_AT")),
                        Status = reader.IsDBNull(reader.GetOrdinal("STATUS")) ? null : reader.GetString(reader.GetOrdinal("STATUS")),
                        
                    };
                }

                return null;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener la observación por ID.", ex.Message);
            }
        }

        public async Task<IEnumerable<Observations>> GetAllByOpporIdAsync(long opporId)
        {
            try
            {
                var list = new List<Observations>();
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_OBSERVATIONS_OPPOR_LIST_BY_OPPOR", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@OPPOR_ID", opporId);

                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var oporId = reader.GetInt64(reader.GetOrdinal("OPPOR_ID"));
                    list.Add(new Observations
                    {
                        ObsId = reader.GetInt64(reader.GetOrdinal("OBS_ID")),
                        BusinessId = reader.IsDBNull(reader.GetOrdinal("BUSINESS_ID")) ? null : reader.GetInt64(reader.GetOrdinal("BUSINESS_ID")),
                        OpporToken = _linkTokenService.Issue("opportunity", oporId, TimeSpan.FromHours(1)),
                        ObsType = reader.IsDBNull(reader.GetOrdinal("OBS_TYPE")) ? null : reader.GetInt32(reader.GetOrdinal("OBS_TYPE")),

                        ObsSeverity = reader.IsDBNull(reader.GetOrdinal("OBS_SEVERITY")) ? null : reader.GetInt32(reader.GetOrdinal("OBS_SEVERITY")),
                        ObsSeverityDesc = reader.IsDBNull(reader.GetOrdinal("OBS_SEVERITY_DESC")) ? null : reader.GetString(reader.GetOrdinal("OBS_SEVERITY_DESC")),
                        ClosedAt = reader.IsDBNull(reader.GetOrdinal("CLOSED_AT")) ? null : reader.GetDateTime(reader.GetOrdinal("CLOSED_AT")),
                        ObsStatusId = reader.IsDBNull(reader.GetOrdinal("OBS_STATUS_ID")) ? null : reader.GetInt64(reader.GetOrdinal("OBS_STATUS_ID")),
                        ObsReason = reader.IsDBNull(reader.GetOrdinal("OBS_REASON")) ? null : reader.GetString(reader.GetOrdinal("OBS_REASON")),
                        DueDate = reader.IsDBNull(reader.GetOrdinal("DUE_DATE")) ? null : reader.GetDateTime(reader.GetOrdinal("DUE_DATE")),
                        DueSetBy = reader.IsDBNull(reader.GetOrdinal("DUE_SET_BY")) ? null : reader.GetInt64(reader.GetOrdinal("DUE_SET_BY")),
                        OpenedBy = reader.IsDBNull(reader.GetOrdinal("OPENED_BY")) ? null : reader.GetInt64(reader.GetOrdinal("OPENED_BY")),
                        OpenedAt = reader.IsDBNull(reader.GetOrdinal("OPENED_AT")) ? null : reader.GetDateTime(reader.GetOrdinal("OPENED_AT")),
                        Status = reader.IsDBNull(reader.GetOrdinal("STATUS")) ? null : reader.GetString(reader.GetOrdinal("STATUS")),
                        OpenedByName = reader.IsDBNull(reader.GetOrdinal("OPENED_BY_NAME")) ? null : reader.GetString(reader.GetOrdinal("OPENED_BY_NAME")),
                        DueSetByName = reader.IsDBNull(reader.GetOrdinal("DUE_SET_BY_NAME")) ? null : reader.GetString(reader.GetOrdinal("DUE_SET_BY_NAME")),
                        ApprovedBy = reader.IsDBNull(reader.GetOrdinal("APPROVED_BY")) ? null : reader.GetInt64(reader.GetOrdinal("APPROVED_BY")),
                        ApprovedDate = reader.IsDBNull(reader.GetOrdinal("APPROVED_DATE")) ? null : reader.GetDateTime(reader.GetOrdinal("APPROVED_DATE")),
                        RejectionReason = reader.IsDBNull(reader.GetOrdinal("REJECTION_REASON")) ? null : reader.GetString(reader.GetOrdinal("REJECTION_REASON")),
                        StateDesc = reader.IsDBNull(reader.GetOrdinal("STATE_DESC")) ? null : reader.GetString(reader.GetOrdinal("STATE_DESC")),
                        StateColor = reader.IsDBNull(reader.GetOrdinal("STATE_COLOR")) ? null : reader.GetString(reader.GetOrdinal("STATE_COLOR")),
                        IsApproved = reader.IsDBNull(reader.GetOrdinal("IS_APPROVED")) ? null : reader.GetBoolean(reader.GetOrdinal("IS_APPROVED")),
                        TypeObsEconomic = reader.IsDBNull(reader.GetOrdinal("TYPE_OBS_ECONOMIC")) ? null : reader.GetInt32(reader.GetOrdinal("TYPE_OBS_ECONOMIC")),
                    });
                }

                return list;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al listar las observaciones de la oportunidad.", ex.Message);
            }
        }


        public async Task<IEnumerable<Observations>> GetAllByProjectAsync(long opporId)
        {
            try
            {
                var list = new List<Observations>();
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_OBSERVATIONS_PROJECT_LIST", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@OPPOR_ID", opporId);

                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var oporId = reader.GetInt64(reader.GetOrdinal("OPPOR_ID"));
                    list.Add(new Observations
                    {
                        ObsId = reader.GetInt64(reader.GetOrdinal("OBS_ID")),
                        BusinessId = reader.IsDBNull(reader.GetOrdinal("BUSINESS_ID")) ? null : reader.GetInt64(reader.GetOrdinal("BUSINESS_ID")),
                        OpporToken = _linkTokenService.Issue("opportunity", oporId, TimeSpan.FromHours(1)),
                        ObsType = reader.IsDBNull(reader.GetOrdinal("OBS_TYPE")) ? null : reader.GetInt32(reader.GetOrdinal("OBS_TYPE")),

                        ObsSeverity = reader.IsDBNull(reader.GetOrdinal("OBS_SEVERITY")) ? null : reader.GetInt32(reader.GetOrdinal("OBS_SEVERITY")),
                        ObsSeverityDesc = reader.IsDBNull(reader.GetOrdinal("OBS_SEVERITY_DESC")) ? null : reader.GetString(reader.GetOrdinal("OBS_SEVERITY_DESC")),
                        ClosedAt = reader.IsDBNull(reader.GetOrdinal("CLOSED_AT")) ? null : reader.GetDateTime(reader.GetOrdinal("CLOSED_AT")),
                        ObsStatusId = reader.IsDBNull(reader.GetOrdinal("OBS_STATUS_ID")) ? null : reader.GetInt64(reader.GetOrdinal("OBS_STATUS_ID")),
                        ObsReason = reader.IsDBNull(reader.GetOrdinal("OBS_REASON")) ? null : reader.GetString(reader.GetOrdinal("OBS_REASON")),
                        DueDate = reader.IsDBNull(reader.GetOrdinal("DUE_DATE")) ? null : reader.GetDateTime(reader.GetOrdinal("DUE_DATE")),
                        DueSetBy = reader.IsDBNull(reader.GetOrdinal("DUE_SET_BY")) ? null : reader.GetInt64(reader.GetOrdinal("DUE_SET_BY")),
                        OpenedBy = reader.IsDBNull(reader.GetOrdinal("OPENED_BY")) ? null : reader.GetInt64(reader.GetOrdinal("OPENED_BY")),
                        OpenedAt = reader.IsDBNull(reader.GetOrdinal("OPENED_AT")) ? null : reader.GetDateTime(reader.GetOrdinal("OPENED_AT")),
                        Status = reader.IsDBNull(reader.GetOrdinal("STATUS")) ? null : reader.GetString(reader.GetOrdinal("STATUS")),
                        OpenedByName = reader.IsDBNull(reader.GetOrdinal("OPENED_BY_NAME")) ? null : reader.GetString(reader.GetOrdinal("OPENED_BY_NAME")),
                        DueSetByName = reader.IsDBNull(reader.GetOrdinal("DUE_SET_BY_NAME")) ? null : reader.GetString(reader.GetOrdinal("DUE_SET_BY_NAME")),
                        ApprovedBy = reader.IsDBNull(reader.GetOrdinal("APPROVED_BY")) ? null : reader.GetInt64(reader.GetOrdinal("APPROVED_BY")),
                        ApprovedDate = reader.IsDBNull(reader.GetOrdinal("APPROVED_DATE")) ? null : reader.GetDateTime(reader.GetOrdinal("APPROVED_DATE")),
                        RejectionReason = reader.IsDBNull(reader.GetOrdinal("REJECTION_REASON")) ? null : reader.GetString(reader.GetOrdinal("REJECTION_REASON")),
                        StateDesc = reader.IsDBNull(reader.GetOrdinal("STATE_DESC")) ? null : reader.GetString(reader.GetOrdinal("STATE_DESC")),
                        StateColor = reader.IsDBNull(reader.GetOrdinal("STATE_COLOR")) ? null : reader.GetString(reader.GetOrdinal("STATE_COLOR")),
                        IsApproved = reader.IsDBNull(reader.GetOrdinal("IS_APPROVED")) ? null : reader.GetBoolean(reader.GetOrdinal("IS_APPROVED"))
                    });
                }

                return list;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al listar las observaciones de la oportunidad.", ex.Message);
            }
        }



        public async Task<IEnumerable<Observations>> GetAllByHiringAsync(long opporId)
        {
            try
            {
                var list = new List<Observations>();
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_OBSERVATIONS_HIRING_LIST", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@OPPOR_ID", opporId);

                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var oporId = reader.GetInt64(reader.GetOrdinal("OPPOR_ID"));
                    list.Add(new Observations
                    {
                        ObsId = reader.GetInt64(reader.GetOrdinal("OBS_ID")),
                        BusinessId = reader.IsDBNull(reader.GetOrdinal("BUSINESS_ID")) ? null : reader.GetInt64(reader.GetOrdinal("BUSINESS_ID")),
                        OpporToken = _linkTokenService.Issue("opportunity", oporId, TimeSpan.FromHours(1)),
                        ObsType = reader.IsDBNull(reader.GetOrdinal("OBS_TYPE")) ? null : reader.GetInt32(reader.GetOrdinal("OBS_TYPE")),

                        ObsSeverity = reader.IsDBNull(reader.GetOrdinal("OBS_SEVERITY")) ? null : reader.GetInt32(reader.GetOrdinal("OBS_SEVERITY")),
                        ObsSeverityDesc = reader.IsDBNull(reader.GetOrdinal("OBS_SEVERITY_DESC")) ? null : reader.GetString(reader.GetOrdinal("OBS_SEVERITY_DESC")),
                        ClosedAt = reader.IsDBNull(reader.GetOrdinal("CLOSED_AT")) ? null : reader.GetDateTime(reader.GetOrdinal("CLOSED_AT")),
                        ObsStatusId = reader.IsDBNull(reader.GetOrdinal("OBS_STATUS_ID")) ? null : reader.GetInt64(reader.GetOrdinal("OBS_STATUS_ID")),
                        ObsReason = reader.IsDBNull(reader.GetOrdinal("OBS_REASON")) ? null : reader.GetString(reader.GetOrdinal("OBS_REASON")),
                        DueDate = reader.IsDBNull(reader.GetOrdinal("DUE_DATE")) ? null : reader.GetDateTime(reader.GetOrdinal("DUE_DATE")),
                        DueSetBy = reader.IsDBNull(reader.GetOrdinal("DUE_SET_BY")) ? null : reader.GetInt64(reader.GetOrdinal("DUE_SET_BY")),
                        OpenedBy = reader.IsDBNull(reader.GetOrdinal("OPENED_BY")) ? null : reader.GetInt64(reader.GetOrdinal("OPENED_BY")),
                        OpenedAt = reader.IsDBNull(reader.GetOrdinal("OPENED_AT")) ? null : reader.GetDateTime(reader.GetOrdinal("OPENED_AT")),
                        Status = reader.IsDBNull(reader.GetOrdinal("STATUS")) ? null : reader.GetString(reader.GetOrdinal("STATUS")),
                        OpenedByName = reader.IsDBNull(reader.GetOrdinal("OPENED_BY_NAME")) ? null : reader.GetString(reader.GetOrdinal("OPENED_BY_NAME")),
                        DueSetByName = reader.IsDBNull(reader.GetOrdinal("DUE_SET_BY_NAME")) ? null : reader.GetString(reader.GetOrdinal("DUE_SET_BY_NAME")),
                        ApprovedBy = reader.IsDBNull(reader.GetOrdinal("APPROVED_BY")) ? null : reader.GetInt64(reader.GetOrdinal("APPROVED_BY")),
                        ApprovedDate = reader.IsDBNull(reader.GetOrdinal("APPROVED_DATE")) ? null : reader.GetDateTime(reader.GetOrdinal("APPROVED_DATE")),
                        RejectionReason = reader.IsDBNull(reader.GetOrdinal("REJECTION_REASON")) ? null : reader.GetString(reader.GetOrdinal("REJECTION_REASON")),
                        StateDesc = reader.IsDBNull(reader.GetOrdinal("STATE_DESC")) ? null : reader.GetString(reader.GetOrdinal("STATE_DESC")),
                        StateColor = reader.IsDBNull(reader.GetOrdinal("STATE_COLOR")) ? null : reader.GetString(reader.GetOrdinal("STATE_COLOR")),
                        IsApproved = reader.IsDBNull(reader.GetOrdinal("IS_APPROVED")) ? null : reader.GetBoolean(reader.GetOrdinal("IS_APPROVED"))
                    });
                }

                return list;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al listar las observaciones de la oportunidad.", ex.Message);
            }
        }

        public async Task<bool> UpdateReasonAsync(Observations entity)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var tx = connection.BeginTransaction();

            try
            {
                using var cmd = new SqlCommand("SP_WS_OBSERVATIONS_OPPOR_UPDATE_REASON", connection, tx)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@OBS_ID", entity.ObsId);
                cmd.Parameters.AddWithValue("@IS_APPROVED", entity.IsApproved);
                if (!string.IsNullOrEmpty(entity.RejectionReason))
                {
                    cmd.Parameters.AddWithValue("@REJECTION_REASON", entity.RejectionReason);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@REJECTION_REASON", DBNull.Value);
                }
                if (entity.UsersBy.HasValue && entity.UsersBy.Value > 0)
                {
                    cmd.Parameters.AddWithValue("@APPROVED_BY", entity.UsersBy.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@APPROVED_BY", DBNull.Value);
                }

                using var reader = await cmd.ExecuteReaderAsync();

                bool success = false;
                if (await reader.ReadAsync())
                {
                    int status = reader.GetInt32(reader.GetOrdinal("status"));
                    success = (status == 1);
                }
                reader.Close();

                if (success) await tx.CommitAsync();
                else await tx.RollbackAsync();

                return success;
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                throw new DatabaseException("Error al resolver la observación.", ex.Message);
            }
        }

        public async Task<bool> UpdateDueDateAsync(Observations entity)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var tx = connection.BeginTransaction();

            try
            {
                using var cmd = new SqlCommand("SP_WS_UPDATE_OBSERVATIONS_DATE", connection, tx)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@OBS_ID", entity.ObsId);

                if (entity.DueDate.HasValue)
                    cmd.Parameters.AddWithValue("@DUE_DATE", entity.DueDate.Value);
                else
                    cmd.Parameters.AddWithValue("@DUE_DATE", DBNull.Value);

                if (entity.ObsStatusId.HasValue)
                    cmd.Parameters.AddWithValue("@OBS_STATUS_ID", entity.ObsStatusId.Value);
                else
                    cmd.Parameters.AddWithValue("@OBS_STATUS_ID", DBNull.Value);
                if (entity.UsersBy > 0) 
                    cmd.Parameters.AddWithValue("@USERS_BY", entity.UsersBy);
                else
                    cmd.Parameters.AddWithValue("@USERS_BY", DBNull.Value);


                using var reader = await cmd.ExecuteReaderAsync();

                bool success = false;
                if (await reader.ReadAsync())
                {
                    int status = reader.GetInt32(reader.GetOrdinal("status"));
                    string dbMessage = reader.GetString(reader.GetOrdinal("message"));

                    if (status == 0)
                    {
                        throw new DatabaseException($"Error SQL: {dbMessage}");
                    }

                    success = (status == 1);
                }
                reader.Close();

                if (success) await tx.CommitAsync();
                else await tx.RollbackAsync();

                return success;
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                throw new DatabaseException("Error en Repositorio: " + ex.Message);
            }
        }



    }
}