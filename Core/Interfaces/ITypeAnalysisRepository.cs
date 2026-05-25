using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ITypeAnalysisRepository
    {
        Task<PagedSelect<OptionItem>> GetTypeAnalysisForSelectAsync(string? search, int page, int pageSize);
    }
}
