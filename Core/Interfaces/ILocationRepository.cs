using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ILocationRepository
    {
        Task<PagedSelect<OptionItem>> GetDepartmentForSelectAsync(string? search, int page, int pageSize);
        Task<PagedSelect<OptionItem>> GetProvinceForSelectAsync(int departmentId, string? search, int page, int pageSize);
        Task<PagedSelect<OptionItem>> GetDistrictForSelectAsync(int departmentId, int provinceId, string? search, int page, int pageSize);
    }
}
