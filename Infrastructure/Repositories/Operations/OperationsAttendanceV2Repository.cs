using Core.Interfaces.Operations;
using Core.Projections.AppAttendance;
using Core.Requests;
using Infrastructure.Persistence;
using System.Data;
using static Core.Requests.AppAttendanceBatchV2Request;

namespace Infrastructure.Repositories.Operations
{
    public class OperationsAttendanceV2Repository(IDapperHelper dapperHelper) : IOperationsAttendanceV2Repository
    {
        private readonly IDapperHelper _dapperHelper = dapperHelper;

        public async Task<AppAttendanceDailyV2Result> GetDailyAsync(long businessId, long userId, DateTime attendanceDate)
        {
            var parameters = DapperParams.From(new
            {
                BusinessId = businessId,
                UserId = userId,
                AttendanceDate = DbParam.Date(attendanceDate)
            });

            return await _dapperHelper.QueryMultipleAsync(
                "SP_WS_GET_APP_ATTENDANCE_DAILY_V2",
                async reader => new AppAttendanceDailyV2Result
                {
                    Operations = await reader.ReadAsync<AppAttendanceOperationProjection>(),
                    Configs    = await reader.ReadAsync<AppAttendanceProjectConfigProjection>(),
                    Workers    = await reader.ReadAsync<AppAttendanceWorkerV2Projection>(),
                    Sessions   = await reader.ReadAsync<AppAttendanceSessionV2Projection>(),
                    Details    = await reader.ReadAsync<AppAttendanceDetailProjection>(),
                    Statuses   = await reader.ReadAsync<AppAttendanceStatusProjection>()
                },
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        public async Task<int> InsertBatchAsync(AppAttendanceBatchV2Request request, long appUserId)
        {
            var parameters = DapperParams.From(new
            {
                request.BusinessId,
                request.OperationsId,
                request.AttendanceDate,
                request.SessionType,
                request.UserId,
                request.SessionStartTime,
                request.SessionEndTime,
                request.GroupPhotoUid
            });

            var table = BuildDetailTable(request.Details);
            parameters.WithTable("Details", table, "dbo.TVP_APP_ATTENDANCE_DETAIL_V2");

            await _dapperHelper.ExecuteAsync("SP_WS_REGISTER_APP_ATTENDANCE_BATCH_V2", parameters, commandType: CommandType.StoredProcedure);

            return request.Details.Count();
        }

        private static DataTable BuildDetailTable(IEnumerable<AppAttendanceBatchV2DetailRequest> details)
        {
            var table = new DataTable();
            table.Columns.Add("ASSIGNMENT_ID", typeof(long)).AllowDBNull = true;
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
                row["ASSIGNMENT_ID"]        = (object?)item.AssignmentId ?? DBNull.Value;
                row["WORKER_ID"]            = item.WorkerId;
                row["ATTENDANCE_STATUS_ID"] = item.AttendanceStatusId;
                row["CHECK_TIME"]           = item.CheckTime;
                row["LATE_MINUTES"]         = (object?)item.LateMinutes ?? DBNull.Value;
                row["EARLY_EXIT_MINUTES"]   = (object?)item.EarlyExitMinutes ?? DBNull.Value;
                row["OBSERVATION"]          = item.Observation ?? (object)DBNull.Value;
                row["PHOTO_UID"]            = (object?)item.PhotoUid ?? DBNull.Value;
                table.Rows.Add(row);
            }

            return table;
        }
    }
}
