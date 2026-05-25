
using Application.DTOs.PaymentType;
using Application.DTOs.StateOpportunity;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.StateOpportunity
{
    public class GetByIdStateOpportunity
    {
        private readonly IStateOpportunityRepository _repository;
        private readonly IMapper _mapper;
        public GetByIdStateOpportunity(IStateOpportunityRepository repository, IMapper mapper)
        {
            _repository = repository;

            _mapper = mapper;
        }
        public async Task<StateOpportunityByIdDto> ExecuteAsync(long stateOpportunityId)
        {
            var entities = await _repository.GetByIdAsync(stateOpportunityId);
            return _mapper.Map<StateOpportunityByIdDto>(entities);
        }
    }
}
