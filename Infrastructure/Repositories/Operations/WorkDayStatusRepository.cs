using Core.Entities;
using Core.Entities.Operations;
using Core.Entities.paginations;
using Core.Interfaces.Operations;
using Core.Projections.Operations;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Operations
{
    public class WorkDayStatusRepository(IDapperHelper dapperHelper) : IWorkDayStatusRepository
    {
        private readonly IDapperHelper _dapperHelper = dapperHelper;
        public async Task<PagedSelect<WorkDayStatusSelectItem?>> GetForSelectAsync(long businessId, int page, int pageSize, string search)
        {
            var parameters = DapperParams.From(new
            {
                BusinessId = businessId,
                PageNumber = page,
                PageSize = pageSize,
                Search = search
            });
            var result = await _dapperHelper.QueryAsync<WorkDayStatusSelectItem>("SP_WS_SELECT_WORKDAY_STATUS", parameters);
            return new PagedSelect<WorkDayStatusSelectItem?>
            {
                Items = result.ToList(),
                Page = page,
                PageSize = pageSize,
                HasMore = false
            };
        }

        public async Task<WorkDayStatus?> GetByIdAsync(long WorkdayStatusId, long businessId)
        {
            var parameters = DapperParams.From(new
            {

                BusinessId = businessId,
                WorkdayStatusId = WorkdayStatusId
            });

            return await _dapperHelper.QueryFirstOrDefaultAsync<WorkDayStatus>("SP_WS_GETBYID_WORKDAY_STATUS", parameters);
        }
    }

}
