using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IAccountLevelRepository
    {
        Task<PagedSelect<OptionItem>> GetAccountLevelForSelectAsync(string? search, int page, int pageSize);
    }
}
