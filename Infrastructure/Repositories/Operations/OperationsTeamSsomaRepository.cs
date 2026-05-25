using Core.Entities.Operations;
using Core.Interfaces.Operations;
using Core.Projections.Operations;
using Core.Results.Operations;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Rows.Operations;
using Microsoft.Data.SqlClient;
using SharedKernel;
using System.Data;

namespace Infrastructure.Repositories.Operations
{
    public class OperationsTeamSsomaRepository(IDapperHelper dapperHelper) : IOperationsTeamSsomaRepository
    {
        private readonly IDapperHelper _dapperHelper = dapperHelper;

        public async Task<OperationsTeamSsomaCreateResult> AddAsync(
            OperationsTeamSsoma entity,
            IEnumerable<OperationsTeamSsomaAssignmentItem> teamSsoma,
            IDbTransaction transaction)
        {
            try
            {
                if (entity is null)
                    throw new ArgumentNullException(nameof(entity));

                var teamItems = teamSsoma?.ToList() ?? new List<OperationsTeamSsomaAssignmentItem>();

                if (!teamItems.Any())
                    throw new ArgumentException("El equipo SSOMA no puede estar vacío.", nameof(teamSsoma));

                var parameters = DapperParams.From(new
                {
                    entity.BusinessId,
                    entity.SsomaProcessId,
                    entity.OperationsProjectConfigId,
                    entity.CreateUser,
                    TeamSsoma = DbParam.Table(
                        BuildTeamSsomaDataTable(teamItems),
                        "dbo.TvpOperationsTeamSsoma")
                })
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                var insertedRowsDb = (await _dapperHelper.QueryAsync<OperationsTeamSsomaInsertedRow>(
                    "SP_WS_INSERT_OPERATIONS_TEAM_SSOMA",
                    parameters,
                    transaction,
                    commandType: CommandType.StoredProcedure)).ToList();

                return new OperationsTeamSsomaCreateResult
                {
                    Response = new GlobalResponse
                    {
                        Status = parameters.Get<int>("@COutput"),
                        Message = parameters.Get<string>("@SOutput"),
                        Id = insertedRowsDb.FirstOrDefault()?.OperationsTeamSsomaId
                    },
                    InsertedRows = insertedRowsDb.Select(x => new OperationsTeamSsomaCreatedItem
                    {
                        OperationsTeamSsomaId = x.OperationsTeamSsomaId,
                        WorkerId = x.WorkerId,
                        SsomaRoleId = x.SsomaRoleId,
                        StartDate = DateOnly.FromDateTime(x.StartDate),
                        EndDate = x.EndDate.HasValue ? DateOnly.FromDateTime(x.EndDate.Value) : null,
                        IsPrimary = x.IsPrimary
                    }).ToList()
                };
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al registrar el equipo SSOMA.", ex.Message);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al guardar el equipo SSOMA.", ex.Message);
            }
        }

        public async Task<OperationsTeamSsomaDetailProjection?> GetByIdAsync(long operationsTeamSsomaId)
            => await GetByIdAsync(operationsTeamSsomaId, transaction: null);

        public async Task<OperationsTeamSsomaDetailProjection?> GetByIdAsync(
            long operationsTeamSsomaId,
            IDbTransaction? transaction)
        {
            try
            {
                const string sql = @"
                    WITH TargetProcess AS (
                        SELECT TOP 1
                            OTS.BUSINESS_ID,
                            OTS.SSOMA_PROCESS_ID
                        FROM dbo.OPERATIONS_TEAM_SSOMA OTS
                        WHERE OTS.OPERATIONS_TEAM_SSOMA_ID = @OperationsTeamSsomaId
                          AND OTS.STATUS = '1'
                    )
                    SELECT
                        OTS.OPERATIONS_TEAM_SSOMA_ID  AS OperationsTeamSsomaId,
                        OTS.BUSINESS_ID               AS BusinessId,
                        OTS.SSOMA_PROCESS_ID          AS SsomaProcessId,
                        OTS.ASSIGNMENT_TYPE_ID        AS AssignmentId,
                        OTS.IS_ACTIVE                 AS IsActive,
                        OTS.REASON_CHANGE             AS ReasonChange,
                        OTS.REPLACED_ASSIGNMENT_ID    AS ReplacedAssignmentId,
                        OTS.CLIENT_APPROVAL_STATUS_ID AS ClientApprovalStatusId,
                        OTS.CLIENT_APPROVAL_DATE      AS ClientApprovalDate,
                        OTS.COMMENTS                  AS Comments,
                        OTS.OPERATIONS_PROJECT_CONFIG_ID AS OperationsProjectConfigId,
                        OTS.WORKER_ID                 AS WorkerId,
                        OTS.SSOMA_ROLE_ID             AS SsomaRoleId,
                        OTS.START_DATE                AS StartDate,
                        OTS.END_DATE                  AS EndDate,
                        OTS.IS_PRIMARY                AS IsPrimary
                    FROM dbo.OPERATIONS_TEAM_SSOMA OTS
                    INNER JOIN TargetProcess TP
                        ON TP.BUSINESS_ID = OTS.BUSINESS_ID
                       AND TP.SSOMA_PROCESS_ID = OTS.SSOMA_PROCESS_ID
                    WHERE OTS.STATUS = '1'
                    ORDER BY OTS.OPERATIONS_TEAM_SSOMA_ID;";

                var rows = (await _dapperHelper.QueryAsync<OperationsTeamSsomaRow>(
                    sql,
                    DapperParams.From(new { OperationsTeamSsomaId = operationsTeamSsomaId }),
                    transaction,
                    commandType: CommandType.Text)).ToList();

                return BuildDetail(rows, operationsTeamSsomaId);
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener el equipo SSOMA por ID.", ex.Message);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al obtener el equipo SSOMA por ID.", ex.Message);
            }
        }

        public async Task<OperationsTeamSsomaDetailProjection?> GetByProcessIdAsync(long businessId, long ssomaProcessId)
            => await GetByProcessIdAsync(businessId, ssomaProcessId, transaction: null);

        public async Task<OperationsTeamSsomaDetailProjection?> GetByProcessIdAsync(
            long businessId,
            long ssomaProcessId,
            IDbTransaction? transaction)
        {
            try
            {
                const string sql = @"
                    SELECT
                        OTS.OPERATIONS_TEAM_SSOMA_ID  AS OperationsTeamSsomaId,
                        OTS.BUSINESS_ID               AS BusinessId,
                        OTS.SSOMA_PROCESS_ID          AS SsomaProcessId,
                        OTS.ASSIGNMENT_TYPE_ID        AS AssignmentId,
                        OTS.IS_ACTIVE                 AS IsActive,
                        OTS.REASON_CHANGE             AS ReasonChange,
                        OTS.REPLACED_ASSIGNMENT_ID    AS ReplacedAssignmentId,
                        OTS.CLIENT_APPROVAL_STATUS_ID AS ClientApprovalStatusId,
                        OTS.CLIENT_APPROVAL_DATE      AS ClientApprovalDate,
                        OTS.COMMENTS                  AS Comments,
                        OTS.OPERATIONS_PROJECT_CONFIG_ID AS OperationsProjectConfigId,
                        OTS.WORKER_ID                 AS WorkerId,
                        OTS.SSOMA_ROLE_ID             AS SsomaRoleId,
                        OTS.START_DATE                AS StartDate,
                        OTS.END_DATE                  AS EndDate,
                        OTS.IS_PRIMARY                AS IsPrimary
                    FROM dbo.OPERATIONS_TEAM_SSOMA OTS
                    WHERE OTS.BUSINESS_ID = @BusinessId
                      AND OTS.SSOMA_PROCESS_ID = @SsomaProcessId
                      AND OTS.STATUS = '1'
                    ORDER BY OTS.OPERATIONS_TEAM_SSOMA_ID;";

                var rows = (await _dapperHelper.QueryAsync<OperationsTeamSsomaRow>(
                    sql,
                    DapperParams.From(new
                    {
                        BusinessId = businessId,
                        SsomaProcessId = ssomaProcessId
                    }),
                    transaction,
                    commandType: CommandType.Text)).ToList();

                return BuildDetail(rows);
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener el equipo SSOMA por proceso.", ex.Message);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al obtener el equipo SSOMA por proceso.", ex.Message);
            }
        }

        public async Task<IEnumerable<OperationsTeamSsomaListItemProjection>> GetListByProcessIdAsync(long businessId, long ssomaProcessId)
        {
            var parameters = DapperParams.From(new
            {
                BusinessId = businessId,
                SsomaProcessId = ssomaProcessId
            });

            return await _dapperHelper.QueryAsync<OperationsTeamSsomaListItemProjection>(
                "SP_WS_GET_LIST_OPERATIONS_TEAM_SSOMA",
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        public async Task<GlobalResponse> UpdateAsync(
            OperationsTeamSsoma entity,
            IEnumerable<OperationsTeamSsomaAssignmentItem> teamSsoma,
            IDbTransaction transaction)
        {
            try
            {
                if (entity is null)
                    throw new ArgumentNullException(nameof(entity));

                var teamItems = teamSsoma?.ToList() ?? new List<OperationsTeamSsomaAssignmentItem>();

                if (!teamItems.Any())
                    throw new ArgumentException("El equipo SSOMA no puede estar vacío.", nameof(teamSsoma));

                var parameters = DapperParams.From(new
                {
                    entity.BusinessId,
                    entity.SsomaProcessId,
                    entity.OperationsProjectConfigId,
                    entity.UpdateUser,
                    TeamSsoma = DbParam.Table(
                        BuildTeamSsomaDataTableUpdate(teamItems),
                        "dbo.TVP_OPERATIONS_TEAM_SSOMA_UPDATE")
                })
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync(
                    "SP_WS_UPDATE_OPERATIONS_TEAM_SSOMA",
                    parameters,
                    transaction,
                    commandType: CommandType.StoredProcedure);

                return new GlobalResponse
                {
                    Status = parameters.Get<int>("@COutput"),
                    Message = parameters.Get<string>("@SOutput")
                };
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar el equipo SSOMA.", ex.Message);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al actualizar el equipo SSOMA.", ex.Message);
            }
        }

        public async Task<GlobalResponse> UpdateTeamSssomaAssignmentStatusAsync(OperationsTeamSsoma entity, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    entity.BusinessId,
                    entity.OperationsTeamSsomaId,
                    entity.SsomaProcessId,
                    entity.ReasonChange,
                    entity.ReplacedAssignmentId,
                    entity.ClientApprovalStatusId,
                    entity.ClientApprovalDate,
                    entity.Comments,
                    entity.UpdateUser

                })
                .WithOutputInt("@COutput");

                await _dapperHelper.ExecuteAsync(
                    "SP_UPDATE_OPERATIONS_TEAM_SSOMA_ASSIGNMENT_STATUS",
                    parameters,
                    transaction,
                    commandType: CommandType.StoredProcedure);

                return new GlobalResponse
                {
                    Status = parameters.Get<int>("@COutput")
                };
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al cambio de estado del assignamiento.", ex.Message);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al cambio de estado del assignamiento.", ex.Message);
            }
        }

        public async Task<GlobalResponse> RelocationTeamSsomaAsync(OperationsTeamSsoma entity, IDbTransaction transaction)
        {
            try
            {

                var parameters = DapperParams.From(new
                {
                    entity.BusinessId,
                    entity.OperationsTeamSsomaId,
                    entity.SsomaProcessId,
                    entity.OperationsProjectConfigId,
                    StartDate = entity.StartDate.ToDateTime(TimeOnly.MinValue),
                    EndDate = ToDbValue(entity.EndDate),
                    entity.IsPrimary,
                    entity.Comments,
                    entity.CreateUser,

                })
                .WithOutputInt("@Id")
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                var insertedRowsDb = (await _dapperHelper.QueryAsync<OperationsTeamSsoma>(
                    "SP_WS_INSERT_OPERATIONS_TEAM_SSOMA_RELOCATION",
                    parameters,
                    transaction,
                    commandType: CommandType.StoredProcedure)).ToList();

                var cOutput = parameters.Get<int>("@COutput");
                var sOutput = parameters.Get<string>("@SOutput");
                var id = (long)parameters.Get<int>("@Id");

                return new GlobalResponse
                {
                    Status = cOutput,
                    Message = sOutput,
                    Id = id
                };
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al registrar el equipo SSOMA.", ex.Message);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al guardar el equipo SSOMA.", ex.Message);
            }
        }

        public async Task<GlobalResponse> UpdateReplacedAssignmentIdAsync(
            long businessId,
            long operationsTeamSsomaId,
            long replacedAssignmentId,
            long updateUser,
            IDbTransaction transaction)
        {
            try
            {
                const string sql = @"
                    UPDATE dbo.OPERATIONS_TEAM_SSOMA
                    SET REPLACED_ASSIGNMENT_ID = @ReplacedAssignmentId,
                       UPDATE_USER = @UpdateUser,
                       UPDATE_DATE = GETDATE()
                    WHERE BUSINESS_ID = @BusinessId
                     AND OPERATIONS_TEAM_SSOMA_ID = @OperationsTeamSsomaId
                     AND STATUS = '1';";                var affectedRows = await _dapperHelper.ExecuteAsync(
                    sql,
                    DapperParams.From(new
                    {
                        BusinessId = businessId,
                        OperationsTeamSsomaId = operationsTeamSsomaId,
                        ReplacedAssignmentId = replacedAssignmentId,
                        UpdateUser = updateUser
                    }),
                    transaction,
                    commandType: CommandType.Text);

                return new GlobalResponse
                {
                    Status = affectedRows > 0 ? 1 : 0
                };
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar la asignación reemplazada.", ex.Message);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al actualizar la asignación reemplazada.", ex.Message);
            }
        }

        public async Task<GlobalResponse> ReplacementTeamSsomaAsync(OperationsTeamSsoma entity, IDbTransaction transaction)
        {
            try
            {

                var parameters = DapperParams.From(new
                {
                    entity.BusinessId,
                    entity.OperationsTeamSsomaId,
                    entity.SsomaProcessId,
                    entity.WorkerId,
                    entity.OperationsProjectConfigId,
                    StartDate = entity.StartDate.ToDateTime(TimeOnly.MinValue),
                    EndDate = ToDbValue(entity.EndDate),
                    entity.IsPrimary,
                    entity.Comments,
                    entity.CreateUser,

                })
                .WithOutputInt("@Id")
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                var insertedRowsDb = (await _dapperHelper.QueryAsync<OperationsTeamSsoma>(
                    "SP_WS_INSERT_OPERATIONS_TEAM_SSOMA_REPLACEMENT",
                    parameters,
                    transaction,
                    commandType: CommandType.StoredProcedure)).ToList();

                var cOutput = parameters.Get<int>("@COutput");
                var sOutput = parameters.Get<string>("@SOutput");
                var id = (long)parameters.Get<int>("@Id");

                return new GlobalResponse
                {
                    Status = cOutput,
                    Message = sOutput,
                    Id = id
                };
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al registrar el equipo SSOMA.", ex.Message);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al guardar el equipo SSOMA.", ex.Message);
            }
        }

        public async Task<ActiveSsomaAssignmentProjection?> GetActiveAssignmentByWorkerIdAsync(long businessId, long workerId)
        {
            try
            {
                var parameters = DapperParams.From(new 
                { 
                    BusinessId = businessId, 
                    WorkerId = workerId 
                });

                return await _dapperHelper.QueryFirstOrDefaultAsync<ActiveSsomaAssignmentProjection>(
                    "SP_WS_GET_ACTIVE_SSOMA_ASSIGNMENT_BY_WORKER",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener la asignación activa del SSOMA desde el SP.", ex.Message);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al obtener la asignación activa del SSOMA.", ex.Message);
            }
        }

        public async Task<List<long>> GetExistingWorkerIdsAsync(
            long businessId,
            long ssomaProcessId,
            IEnumerable<long> workerIds,
            IDbTransaction transaction)
        {
            var ids = workerIds?.Distinct().ToArray() ?? Array.Empty<long>();

            if (ids.Length == 0)
                return new List<long>();

            const string sql = @"
                SELECT OTS.WORKER_ID
                FROM dbo.OPERATIONS_TEAM_SSOMA OTS
                WHERE OTS.BUSINESS_ID = @BusinessId
                  AND OTS.SSOMA_PROCESS_ID = @SsomaProcessId
                  AND OTS.STATUS = '1'
                  AND ISNULL(OTS.IS_ACTIVE, 1) = 1
                  AND OTS.WORKER_ID IN @WorkerIds;";

            var result = await _dapperHelper.QueryAsync<long>(
                sql,
                DapperParams.From(new
                {
                    BusinessId = businessId,
                    SsomaProcessId = ssomaProcessId,
                    WorkerIds = ids
                }),
                transaction,
                commandType: CommandType.Text);

            return result.ToList();
        }

        public async Task<GlobalResponse> DeleteAsync(
            long operationsTeamSsomaId,
            long userId,
            long businessId,
            IDbTransaction? transaction = null)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    BusinessId = businessId,
                    OperationsTeamSsomaId = operationsTeamSsomaId,
                    UpdateUser = userId
                })
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync(
                    "SP_WS_DELETE_OPERATIONS_TEAM_SSOMA_ASSIGNMENT",
                    parameters,
                    transaction,
                    commandType: CommandType.StoredProcedure);

                return new GlobalResponse
                {
                    Status = parameters.Get<int>("@COutput"),
                    Message = parameters.Get<string>("@SOutput")
                };
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al realizar la eliminación lógica de la asignación SSOMA.", ex.Message);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al realizar la eliminación lógica.", ex.Message);
            }
        }

        private static OperationsTeamSsomaDetailProjection? BuildDetail(List<OperationsTeamSsomaRow> rows, long? targetId = null)
        {
            if (!rows.Any())
                return null;

            // Buscamos el registro que coincida con el ID solicitado (targetId).
            // Si no se especifica, tomamos el primero (comportamiento anterior).
            var baseRecord = targetId.HasValue 
                ? rows.FirstOrDefault(x => x.OperationsTeamSsomaId == targetId.Value) ?? rows.First()
                : rows.First();

            return new OperationsTeamSsomaDetailProjection
            {
                OperationsTeamSsomaId = baseRecord.OperationsTeamSsomaId,
                BusinessId = baseRecord.BusinessId,
                SsomaProcessId = baseRecord.SsomaProcessId,
                OperationsProjectConfigId = baseRecord.OperationsProjectConfigId,
                AssignmentId = baseRecord.AssignmentId,
                IsActive = baseRecord.IsActive,
                ReasonChange = baseRecord.ReasonChange,
                ReplacedAssignmentId = baseRecord.ReplacedAssignmentId,
                ClientApprovalStatusId = baseRecord.ClientApprovalStatusId,
                ClientApprovalDate = baseRecord.ClientApprovalDate,
                Comments = baseRecord.Comments,
                TeamSsoma = rows.Select(x => new OperationsTeamSsomaAssignmentItem
                {
                    OperationsTeamSsomaId = x.OperationsTeamSsomaId,
                    WorkerId = x.WorkerId,
                    SsomaRoleId = x.SsomaRoleId,
                    StartDate = DateOnly.FromDateTime(x.StartDate),
                    EndDate = x.EndDate.HasValue ? DateOnly.FromDateTime(x.EndDate.Value) : null,
                    IsPrimary = x.IsPrimary == 1,
                    OperationsProjectConfigId = x.OperationsProjectConfigId
                }).ToList()
            };
        }

        private static DataTable BuildTeamSsomaDataTable(IEnumerable<OperationsTeamSsomaAssignmentItem> teamSsoma) =>
            BuildTeamSsomaTable(
                teamSsoma,
                keyColumnName: "WORKER_ID",
                keySelector: static item => ToDbValue(item.WorkerId));

        private static DataTable BuildTeamSsomaDataTableUpdate(IEnumerable<OperationsTeamSsomaAssignmentItem> teamSsoma) =>
            BuildTeamSsomaTable(
                teamSsoma,
                keyColumnName: "OPERATIONS_TEAM_SSOMA_ID",
                keySelector: static item => ToDbValue(item.OperationsTeamSsomaId));

        private static DataTable BuildTeamSsomaTable(
            IEnumerable<OperationsTeamSsomaAssignmentItem>? teamSsoma,
            string keyColumnName,
            Func<OperationsTeamSsomaAssignmentItem, object> keySelector)
        {
            var table = new DataTable();
            table.Columns.Add(keyColumnName, typeof(long));
            table.Columns.Add("SSOMA_ROLE_ID", typeof(int));
            table.Columns.Add("START_DATE", typeof(DateTime));
            table.Columns.Add("END_DATE", typeof(DateTime));
            table.Columns.Add("IS_PRIMARY", typeof(int));
            table.Columns.Add("OPERATIONS_PROJECT_CONFIG_ID", typeof(long));

            foreach (var item in teamSsoma ?? Enumerable.Empty<OperationsTeamSsomaAssignmentItem>())
            {
                table.Rows.Add(
                    keySelector(item),
                    item.SsomaRoleId,
                    item.StartDate.ToDateTime(TimeOnly.MinValue),
                    ToDbValue(item.EndDate),
                    item.IsPrimary ? 1 : 0,
                    ToDbValue(item.OperationsProjectConfigId));
            }

            return table;
        }

        private static object ToDbValue(long? value) => value.HasValue ? value.Value : DBNull.Value;

        private static object ToDbValue(DateOnly? value) =>
            value.HasValue
                ? value.Value.ToDateTime(TimeOnly.MinValue)
                : DBNull.Value;
    }
}
