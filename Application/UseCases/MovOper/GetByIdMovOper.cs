using Application.DTOs.MovOper;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.MovOper
{
    public class GetMovOperById
    {
        private readonly IMovOperRepository _repository;
        private readonly IMapper _mapper;

        public GetMovOperById(IMovOperRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<MovOperResponseDto> ExecuteAsync(long movOperId)
        {
            var entity = await _repository.GetByIdAsync(movOperId);
            return _mapper.Map<MovOperResponseDto>(entity);
        }
    }
}
