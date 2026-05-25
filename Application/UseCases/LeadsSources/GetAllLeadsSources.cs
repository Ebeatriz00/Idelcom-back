using Application.DTOs.LeadsSources;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.LeadsSources
{
    public class GetAllLeadsSources
    {
        private readonly ILeadsSourcesRepository _repository;
        private readonly IMapper _mapper;
        public GetAllLeadsSources(ILeadsSourcesRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<PagedResult<LeadsSourcesResponseDto>> ExecuteAsync(long businessId, string? search, int page, int pageSize, long? usersBy)
        {
            var entities = await _repository.GetAllAsync(businessId, search, page, pageSize, usersBy);
            return _mapper.Map<PagedResult<LeadsSourcesResponseDto>>(entities);
        }
    }
}
