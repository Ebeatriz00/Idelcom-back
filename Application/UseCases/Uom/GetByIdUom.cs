using Application.DTOs.Uom;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Uom
{
    public class GetByIdUom
    {
        private readonly IUomRepository _repository;
        private readonly IMapper _mapper;

        public GetByIdUom(IUomRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<UomResponseDto> ExecuteAsync(long uomId)
        {
            var entity = await _repository.GetByIdAsync(uomId);
            return _mapper.Map<UomResponseDto>(entity);
        }
    }
}
