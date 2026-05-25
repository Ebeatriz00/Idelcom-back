using Application.DTOs.Opportunities;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Opportunities
{
    public class GetByIdStateOpportunities
    {
        private readonly IOpportunitiesRepository _repository;
        private readonly IMapper _mapper;
        public GetByIdStateOpportunities(IOpportunitiesRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<OpportunitiesStateGetByIdDto> ExecuteAsync(string linkToken)
        {
            var entity = await _repository.GetStateByIdAsync(linkToken);
            return _mapper.Map<OpportunitiesStateGetByIdDto>(entity);
        }
    }
}
