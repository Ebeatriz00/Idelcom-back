using AutoMapper;
using Core.Entities;
using Core.Entities.paginations;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Bank
{
    public class GetSelectBank
    {
        private readonly IBankRepository _bankRepository;
        private readonly IMapper _mapper;
        public GetSelectBank(IBankRepository bankRepository, IMapper mapper)
        {
            _bankRepository = bankRepository;
            _mapper = mapper;
        }
        public async Task<PagedSelect<OptionItem>> ExecuteAsync( long businessId, string? search, int page, int pageSize)
        {
            var entities = await _bankRepository.GetBanksForSelectAsync(businessId, search, page, pageSize);
            return _mapper.Map<PagedSelect<OptionItem>>(entities);
        }
    }
}
