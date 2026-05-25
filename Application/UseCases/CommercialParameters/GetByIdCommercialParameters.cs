using Application.DTOs.CommercialParameters;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.CommercialParameters
{
    public class GetByIdCommercialParameters
    {
        private readonly ICommercialParametersRepository _repository;
        private readonly IMapper _mapper;

        public GetByIdCommercialParameters(ICommercialParametersRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<ResponseCommercialParametersDto> ExecuteAsync(int commercialParametersId)
        {
            var entity = await _repository.GetByIdAsync(commercialParametersId);
            return _mapper.Map<ResponseCommercialParametersDto>(entity);
        }
    }
}
