using Application.DTOs.MovVis;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.MovVis
{
    public class GetMovVisById
    {
        private readonly IMovVisRepository _repository;
        private readonly IMapper _mapper;

        public GetMovVisById(IMovVisRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<MovVisResponseDto> ExecuteAsync(long movVisId)
        {
            var entity = await _repository.GetByIdAsync(movVisId);
            return _mapper.Map<MovVisResponseDto>(entity);
        }
    }
}
