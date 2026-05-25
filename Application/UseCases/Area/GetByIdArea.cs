using Application.DTOs.Area;
using Application.Exceptions;
using AutoMapper;
using Core.Interfaces;
using Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Area
{
    public class GetByIdArea
    {
        private readonly IAreaRepository _repository;
        private readonly IMapper _mapper;

        public GetByIdArea(IAreaRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<AreaResponseDto> ExecuteAsync(long areasId)
        {
            var entities = await _repository.GetByIdAsync(areasId);
            return _mapper.Map<AreaResponseDto>(entities);
        }
    }

}
