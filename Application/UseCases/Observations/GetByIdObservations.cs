using Application.DTOs.Observations;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Observations
{
    public class GetObservationsById
    {
        private readonly IObservationsRepository _repository;
        private readonly IMapper _mapper;

        public GetObservationsById(IObservationsRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ObservationsResponseDto> ExecuteAsync(long obsId)
        {
            var entities = await _repository.GetByIdAsync(obsId);
            return _mapper.Map<ObservationsResponseDto>(entities);
        }
    }
}
