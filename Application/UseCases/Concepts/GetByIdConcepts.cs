using Application.DTOs.Concepts;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Concepts
{
    public class GetConceptsById
    {
        private readonly IConceptsRepository _repository;
        private readonly IMapper _mapper;

        public GetConceptsById(IConceptsRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ConceptsByIdDto> ExecuteAsync(long conceptsId)
        {
            var entity = await _repository.GetByIdAsync(conceptsId);
            return _mapper.Map<ConceptsByIdDto>(entity);
        }
    }
}
