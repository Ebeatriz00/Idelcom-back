using Application.DTOs.ConceptGroups;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.ConceptGroups
{
    public class GetConceptGroupsById
    {
        private readonly IConceptGroupsRepository _repository;
        private readonly IMapper _mapper;

        public GetConceptGroupsById(IConceptGroupsRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ConceptGroupsResponseDto> ExecuteAsync(long conceptGroupsId)
        {
            var entity = await _repository.GetByIdAsync(conceptGroupsId);
            return _mapper.Map<ConceptGroupsResponseDto>(entity);
        }
    }
}
