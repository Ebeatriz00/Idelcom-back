using Application.DTOs.Concepts;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Concepts
{
    public class GetAllConcepts
    {
        private readonly IConceptsRepository _repository;
        private readonly IMapper _mapper;

        public GetAllConcepts(IConceptsRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PagedResult<ConceptsResponseDto>> ExecuteAsync(long businessId, string? search, int page, int pageSize, long? usersBy)
        {
            var entities = await _repository.GetAllAsync(businessId, search, page, pageSize, usersBy);
            return _mapper.Map<PagedResult<ConceptsResponseDto>>(entities);
        }
    }
}
