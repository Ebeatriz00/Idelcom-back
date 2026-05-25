using Core.Projections.AppAttendance;
using Core.Projections.Operations;
using Core.Requests;

namespace Core.Interfaces.Operations
{
    public interface IOperationsAttendanceRepository
    {
        Task<AttendanceMatrixResult> GetMatrixAsync(
            long businessId,
            DateTime startDate,
            DateTime endDate,
            long? opporId,
            long? workOrderId,
            long? squadId,
            string? search,
            int? statusId,
            int page,
            int pageSize);

        Task<AppAttendanceDailyResult> GetDailyAsync(long businessId, long responsibleWorkerId, DateTime attendanceDate);
        Task<int> InsertBatchAsync(AppAttendanceBatchRequest request, long appUserId);
    }
}
