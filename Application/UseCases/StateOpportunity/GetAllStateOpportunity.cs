
using Application.DTOs.PaymentType;
using Application.DTOs.StateOpportunity;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.StateOpportunity
{
    public class GetAllStateOpportunity
    {
        private readonly IStateOpportunityRepository _repository;
        private readonly IMapper _mapper;

        public GetAllStateOpportunity(IStateOpportunityRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<PagedResult<StateOpportunityResponseDto>> ExecuteAsync(int businessId, string? search, int page, int pageSize, long? usersBy)
        {
            var entities = await _repository.GetAllAsync(businessId, search, page, pageSize, usersBy);
            return _mapper.Map<PagedResult<StateOpportunityResponseDto>>(entities);
        }
    }
}
