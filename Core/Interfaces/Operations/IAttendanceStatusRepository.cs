using Core.Entities.Operations;
using Core.Entities.paginations;
using Core.Projections.Operations;

namespace Core.Interfaces.Operations
{
    public interface IAttendanceStatusRepository
    {
        Task<PagedSelect<AttendanceStatusSelectItem?>> GetAllAsync(long businessId, int page, int pageSize, string? search);
        Task<AttendanceStatus?> GetByIdAsync(long attendanceStatusId, long businessId);
    }

}
