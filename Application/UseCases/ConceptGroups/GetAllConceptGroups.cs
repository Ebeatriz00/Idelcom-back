using Application.DTOs.ConceptGroups;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.ConceptGroups
{
    public class GetAllConceptGroups
    {
        private readonly IConceptGroupsRepository _repository;
        private readonly IMapper _mapper;

        public GetAllConceptGroups(IConceptGroupsRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PagedResult<ConceptGroupsResponseDto>> ExecuteAsync(long businessId, string? search, int page, int pageSize, long? usersBy)
        {
            var entities = await _repository.GetAllAsync(businessId, search, page, pageSize, usersBy);
            return _mapper.Map<PagedResult<ConceptGroupsResponseDto>>(entities);
        }
    }
}
