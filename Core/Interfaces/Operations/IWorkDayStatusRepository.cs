using Core.Entities.Operations;
using Core.Entities.paginations;
using Core.Projections.Operations;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Operations
{
    public interface IWorkDayStatusRepository
    {
        Task<PagedSelect<WorkDayStatusSelectItem?>> GetForSelectAsync(long businessId, int page, int pageSize, string search);
        Task<WorkDayStatus?> GetByIdAsync(long WorkdayStatusId, long businessId);
    }
}
