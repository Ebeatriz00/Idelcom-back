using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.ConceptType
{
    using Application.DTOs.ConceptType;
    using AutoMapper;
    using Core.Entities.paginations;
    using Core.Interfaces;

    

    public class GetAllConceptType
    {
        private readonly IConceptTypeRepository _repository;
        private readonly IMapper _mapper;

        public GetAllConceptType(IConceptTypeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<PagedResult<ConceptTypeResponseDto>> ExecuteAsync(long businessId, string? search, int page, int pageSize)
        {
            var entities = await _repository.GetAllAsync(businessId, search, page, pageSize);
            return _mapper.Map<PagedResult<ConceptTypeResponseDto>>(entities);
        }
    }
}
