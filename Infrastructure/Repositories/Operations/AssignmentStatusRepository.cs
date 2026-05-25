using Core.Entities.Operations;
using Core.Entities.paginations;
using Core.Interfaces.Operations;
using Core.Projections.Operations;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories.Operations
{
    public class AssignmentStatusRepository(IDapperHelper dapperHelper) : IAssignmentStatusRepository
    {
        private readonly IDapperHelper _dapperHelper = dapperHelper;

        public async Task<PagedSelect<AssignmentStatusSelectItem?>> GetForSelectAsync(long businessId, int page, int pageSize, string? search)
        {
            var parameters = DapperParams.From(new
            {
                BusinessId = businessId,
                PageNumber = page,
                PageSize = pageSize,
                Search = search
            });

            var result = await _dapperHelper.QueryAsync<AssignmentStatusSelectItem>("SP_WS_SELECT_ASSIGNMENT_STATUS", parameters);

            return new PagedSelect<AssignmentStatusSelectItem?>
            {
                Items = result.ToList(),
                Page = page,
                PageSize = pageSize,
                HasMore = false
            };
        }

        public async Task<AssignmentStatus?> GetByIdAsync(long assignmentStatusId, long businessId)
        {
            var parameters = DapperParams.From(new
            {
                BusinessId = businessId,
                AssignmentStatusId = assignmentStatusId
            });

            return await _dapperHelper.QueryFirstOrDefaultAsync<AssignmentStatus>("SP_WS_GETBYID_ASSIGNMENT_STATUS", parameters);
        }
    }

}