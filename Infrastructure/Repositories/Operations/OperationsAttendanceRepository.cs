using Core.Interfaces.Operations;
using Core.Projections.AppAttendance;
using Core.Projections.Operations;
using Core.Requests;
using Infrastructure.Persistence;
using System.Data;
using static Core.Requests.AppAttendanceBatchRequest;

namespace Infrastructure.Repositories.Operations
{
    public class OperationsAttendanceRepository(IDapperHelper dapperHelper) : IOperationsAttendanceRepository
    {
        private readonly IDapperHelper _dapperHelper = dapperHelper;

        public async Task<AttendanceMatrixResult> GetMatrixAsync(
            long businessId,
            DateTime startDate,
            DateTime endDate,
            long? opporId,
            long? workOrderId,
            long? squadId,
            string? search,
            int? statusId,
            int page,
            int pageSize)
        {
            var parameters = DapperParams.From(new
            {
                BusinessId = businessId,
                StartDate = startDate,
                EndDate = endDate,
                OpporId = opporId,
                WorkOrderId = workOrderId,
                SquadId = squadId,
                Search = search,
                StatusId = statusId,
                PageNumber = page,
                PageSize = pageSize
            });

            return await _dapperHelper.QueryMultipleAsync(
                "SP_WS_LIST_ATTENDANCE_MATRIX",
                async reader => new AttendanceMatrixResult
                {
                    Projects = await reader.ReadAsync<AttendanceMatrixProjectProjection>(),
                    WorkOrders = await reader.ReadAsync<AttendanceMatrixWorkOrderProjection>(),
                    Squads = await reader.ReadAsync<AttendanceMatrixSquadProjection>(),
                    Details = await reader.ReadAsync<AttendanceMatrixDetailProjection>(),
                    TotalWorkers = await reader.ReadFirstOrDefaultAsync<int>()
                },
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        public async Task<AppAttendanceDailyResult> GetDailyAsync(long businessId, long userId, DateTime attendanceDate)
        {
            var parameters = DapperParams.From(new
            {
                BusinessId = businessId,
                UserId = userId,
                AttendanceDate = DbParam.Date(attendanceDate)
            });

            return await _dapperHelper.QueryMultipleAsync(
                "SP_WS_GET_APP_ATTENDANCE_DAILY",
                async reader => new AppAttendanceDailyResult
                {
                    Operations = await reader.ReadAsync<AppAttendanceOperationProjection>(),
                    Configs = await reader.ReadAsync<AppAttendanceProjectConfigProjection>(),
                    WorkOrders = await reader.ReadAsync<AppAttendanceWorkOrderProjection>(),
                    Squads = await reader.ReadAsync<AppAttendanceSquadProjection>(),
                    Workers = await reader.ReadAsync<AppAttendanceWorkerProjection>(),
                    Sessions = await reader.ReadAsync<AppAttendanceSessionProjection>(),
                    Details = await reader.ReadAsync<AppAttendanceDetailProjection>(),
                    Statuses = await reader.ReadAsync<AppAttendanceStatusProjection>()
                },
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        public async Task<int> InsertBatchAsync(AppAttendanceBatchRequest request, long appUserId)
        {
            var parameters = BuildBaseParameters(request);
            var table = BuildDetailTable(request.Details);

            parameters.WithTable("Details", table, "dbo.TVP_APP_ATTENDANCE_DETAIL");

            await _dapperHelper.ExecuteAsync("SP_WS_REGISTER_APP_ATTENDANCE_BATCH", parameters, commandType: CommandType.StoredProcedure);

            return request.Details.Count();
        }

        #region Helpers Privados

        private static DapperParams BuildBaseParameters(AppAttendanceBatchRequest request)
        {
            return DapperParams.From(new
            {
                request.BusinessId,
                request.WorkOrderId,
                request.SquadId,
                request.AttendanceDate,
                request.SessionType,
                request.UserId,
                request.SessionStartTime,
                request.SessionEndTime,
                request.GroupPhotoUid

            });
        }

        private static DataTable BuildDetailTable(IEnumerable<AppAttendanceBatchDetailRequest> details)
        {
            var table = new DataTable();
            table.Columns.Add("ASSIGNMENT_ID", typeof(long));
            table.Columns.Add("WORKER_ID", typeof(long));
            table.Columns.Add("ATTENDANCE_STATUS_ID", typeof(int)).AllowDBNull = false;
            table.Columns.Add("CHECK_TIME", typeof(DateTime)).AllowDBNull = false;
            table.Columns.Add("LATE_MINUTES", typeof(int)).AllowDBNull = true;
            table.Columns.Add("EARLY_EXIT_MINUTES", typeof(int)).AllowDBNull = true;
            table.Columns.Add("OBSERVATION", typeof(string)).AllowDBNull = true;
            table.Columns.Add("PHOTO_UID", typeof(Guid)).AllowDBNull = true;

            foreach (var item in details)
            {
                var row = table.NewRow();
                row["ASSIGNMENT_ID"] = item.AssignmentId;
                row["WORKER_ID"] = item.WorkerId;
                row["ATTENDANCE_STATUS_ID"] = item.AttendanceStatusId;
                row["CHECK_TIME"] = item.CheckTime;
                row["LATE_MINUTES"] = (object?)item.LateMinutes ?? DBNull.Value;
                row["EARLY_EXIT_MINUTES"] = (object?)item.EarlyExitMinutes ?? DBNull.Value;
                row["OBSERVATION"] = item.Observation ?? (object)DBNull.Value;
                row["PHOTO_UID"] = (object?)item.PhotoUid ?? DBNull.Value;

                table.Rows.Add(row);
            }
            return table;
        }

        #endregion
    }
}
