using Application.DTOs.Sector;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Sector
{
    public class GetSectorById
    {
        private readonly ISectorRepository _repository;
        private readonly IMapper _mapper;
        public GetSectorById(ISectorRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<SectorResponseDto> ExecuteAsync(long SectorId)
        {
            var entity = await _repository.GetByIdAsync(SectorId);
            return _mapper.Map<SectorResponseDto>(entity);
        }
    }
}
