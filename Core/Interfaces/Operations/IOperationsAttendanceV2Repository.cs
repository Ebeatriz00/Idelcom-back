using Core.Projections.AppAttendance;
using Core.Requests;

namespace Core.Interfaces.Operations
{
    public interface IOperationsAttendanceV2Repository
    {
        Task<AppAttendanceDailyV2Result> GetDailyAsync(long businessId, long userId, DateTime attendanceDate);
        Task<int> InsertBatchAsync(AppAttendanceBatchV2Request request, long appUserId);
    }
}
