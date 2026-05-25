using Application.DTOs.MovClas;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.MovClas
{
    public class GetMovClasById
    {
        private readonly IMovClasRepository _repository;
        private readonly IMapper _mapper;

        public GetMovClasById(IMovClasRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<MovClasResponseDto> ExecuteAsync(long movClasId)
        {
            var entity = await _repository.GetByIdAsync(movClasId);
            return _mapper.Map<MovClasResponseDto>(entity);
        }
    }
}
