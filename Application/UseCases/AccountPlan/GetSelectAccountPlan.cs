using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.AccountPlan
{
    public class GetSelectAccountPlan
    {
        private readonly IAccountPlanRepository _accountPlanRepository;
        private readonly IMapper _mapper;
        public GetSelectAccountPlan(IAccountPlanRepository accountPlanRepository, IMapper mapper)
        {
            _accountPlanRepository = accountPlanRepository;
            _mapper = mapper;
        }
        public async Task<PagedSelect<OptionItem>> ExecuteAsync(long businessId, string? search, int page, int pageSize)
        {
            var entities = await _accountPlanRepository.GetAccountPlanForSelectAsync(businessId, search, page, pageSize);
            return _mapper.Map<PagedSelect<OptionItem>>(entities);
        }
    }
}
