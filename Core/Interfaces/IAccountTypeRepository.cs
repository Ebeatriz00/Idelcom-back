using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IAccountTypeRepository
    {
        Task<PagedSelect<OptionItem>> GetAccountTypeForSelectAsync(string? search, int page, int pageSize);
    }
}
