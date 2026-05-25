using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.AccountLevel
{
    public class GetSelectAccountLevel
    {
        private readonly IAccountLevelRepository _accountLevelRepository;
        private readonly IMapper _mapper;
        public GetSelectAccountLevel(IAccountLevelRepository accountLevelRepository, IMapper mapper)
        {
            _accountLevelRepository = accountLevelRepository;
            _mapper = mapper;
        }
        public async Task<PagedSelect<OptionItem>> ExecuteAsync(string? search, int page, int pageSize)
        {
            var entities = await _accountLevelRepository.GetAccountLevelForSelectAsync(search, page, pageSize);
            return _mapper.Map<PagedSelect<OptionItem>>(entities);
        }
    }
}
