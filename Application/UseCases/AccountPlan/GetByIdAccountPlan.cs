using Application.DTOs.AccountPlan;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.AccountPlan
{
    public class GetByIdAccountPlan
    {
        private readonly IAccountPlanRepository _repository;
        private readonly IMapper _mapper;

        public GetByIdAccountPlan(IAccountPlanRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<AccountPlanByIdDto> ExecuteAsync(long accountPlanId)
        {
            var entity = await _repository.GetByIdAsync(accountPlanId);
            return _mapper.Map<AccountPlanByIdDto>(entity);
        }
    }
}
