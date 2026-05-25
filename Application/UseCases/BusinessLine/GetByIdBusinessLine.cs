using Application.DTOs.BusinessLine;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.BusinessLine
{
    public class GetByIdBusinessLine
    {
        private readonly IBusinessLineRepository _repository;
        private readonly IMapper _mapper;

        public GetByIdBusinessLine(IBusinessLineRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<BusinessLineResponseDto> ExecuteAsync(long BusinessLineId)
        {
            var entities = await _repository.GetByIdAsync(BusinessLineId);
            return _mapper.Map<BusinessLineResponseDto>(entities);
        }
    }
}
