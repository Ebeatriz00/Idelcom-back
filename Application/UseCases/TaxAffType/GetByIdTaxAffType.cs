using Application.DTOs.TaxAffType;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.TaxAffType
{
    public class GetByIdTaxAffType
    {
        private readonly ITaxAffTypeRepository _repository;
        private readonly IMapper _mapper;
        public GetByIdTaxAffType(ITaxAffTypeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<TaxAffTypeResponseDto> ExecuteAsync(long taxAffTypeId)
        {
            var entities = await _repository.GetByIdAsync(taxAffTypeId);
            return _mapper.Map<TaxAffTypeResponseDto>(entities);
        }
    }
}
