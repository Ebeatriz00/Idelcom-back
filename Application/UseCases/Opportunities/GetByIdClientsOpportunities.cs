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
    public class GetByIdClientsOpportunities
    {
        private readonly IOpportunitiesRepository _repository;
        private readonly IMapper _mapper;
        public GetByIdClientsOpportunities(IOpportunitiesRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<OpportunitiesClientsGetByIdDto> ExecuteAsync(string linkToken)
        {
            var entity = await _repository.GetClientsByIdAsync(linkToken);
            return _mapper.Map<OpportunitiesClientsGetByIdDto>(entity);
        }
    }
}
