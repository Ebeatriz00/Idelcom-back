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
    public class GetDetailOpportunities
    {
        private readonly IOpportunitiesRepository _repository;
        private readonly IMapper _mapper;

        public GetDetailOpportunities(IOpportunitiesRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<OpportunitiesDetailDto> ExecuteAsync(string linkToken, long businessId, long? usersId, CancellationToken ct = default)
        {
            var entities = await _repository.GetDetailAsync(linkToken, businessId, usersId, ct);
            return _mapper.Map<OpportunitiesDetailDto>(entities);
        }
    }
}
