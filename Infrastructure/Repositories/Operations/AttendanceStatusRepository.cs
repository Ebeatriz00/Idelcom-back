using Core.Entities.Operations;
using Core.Entities.paginations;
using Core.Interfaces.Operations;
using Core.Projections.Operations;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories.Operations
{
    public class AttendanceStatusRepository(IDapperHelper dapperHelper) : IAttendanceStatusRepository
    {
        private readonly IDapperHelper _dapperHelper = dapperHelper;

        public async Task<PagedSelect<AttendanceStatusSelectItem?>> GetAllAsync(long businessId, int page, int pageSize, string? search)
        {
            var parameters = DapperParams.From(new
            {
                BusinessId = businessId,
                PageNumber = page,
                PageSize = pageSize,
                Search = search
            });

            var result = await _dapperHelper.QueryAsync<AttendanceStatusSelectItem?>("SP_WS_SELECT_ATTENDANCE_STATUS", parameters);

            return new PagedSelect<AttendanceStatusSelectItem?>
            {
                Items = result.ToList(),
                Page = page,
                PageSize = pageSize,
                HasMore = false
            };
        }

        public async Task<AttendanceStatus?> GetByIdAsync(long attendanceStatusId, long businessId)
        {
            var parameters = DapperParams.From(new
            {
                BusinessId = businessId,
                AttendanceStatusId = attendanceStatusId
            });

            return await _dapperHelper.QueryFirstOrDefaultAsync<AttendanceStatus>("SP_WS_GETBYID_ATTENDANCE_STATUS", parameters);
        }

    }

}
