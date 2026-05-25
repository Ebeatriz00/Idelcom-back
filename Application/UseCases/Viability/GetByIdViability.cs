using Application.DTOs.Viability;
using Application.Exceptions;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Viability
{
    public class GetByIdViability
    {
        private readonly IViabilityRepository _repository;
        private readonly IMapper _mapper;

        public GetByIdViability(IViabilityRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<ViabilityGetByIdDto> ExecuteAsync(string linkToken)
        {
            var entity = await _repository.GetByIdAsync(linkToken);
            return _mapper.Map<ViabilityGetByIdDto>(entity);
        }
    }
}
