using Application.DTOs.MovSunat;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.MovSunat
{
    public class GetMovSunatById
    {
        private readonly IMovSunatRepository _repository;
        private readonly IMapper _mapper;

        public GetMovSunatById(IMovSunatRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<MovSunatResponseDto> ExecuteAsync(long movSunatId)
        {
            var entity = await _repository.GetByIdAsync(movSunatId);
            return _mapper.Map<MovSunatResponseDto>(entity);
        }
    }
}
