using Application.DTOs.ConceptType;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.ConceptType
{

    public class GetByIdConceptType
    {
        private readonly IConceptTypeRepository _repository;
        private readonly IMapper _mapper;

        public GetByIdConceptType(IConceptTypeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ConceptTypeResponseDto> ExecuteAsync(long conceptTypeId)
        {
            var entity = await _repository.GetByIdAsync(conceptTypeId);
            return _mapper.Map<ConceptTypeResponseDto>(entity);
        }
    }
}
