using Application.DTOs.MovPer;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.MovPer
{
    public class GetMovPerById
    {
        private readonly IMovPerRepository _repository;
        private readonly IMapper _mapper;

        public GetMovPerById(IMovPerRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<MovPerResponseDto> ExecuteAsync(long movPerId)
        {
            var entity = await _repository.GetByIdAsync(movPerId);
            return _mapper.Map<MovPerResponseDto>(entity);
        }
    }
}
