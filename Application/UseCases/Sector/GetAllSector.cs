using Application.DTOs.Sector;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Sector
{
    public class GetAllSector
    {
        private readonly ISectorRepository _repository;
        private readonly IMapper _mapper;
        public GetAllSector(ISectorRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<PagedResult<SectorResponseDto>> ExecuteAsync(long businessId, string? search, int page, int pageSize)
        {
            var entities = await _repository.GetAllAsync(businessId, search, page, pageSize);
            return _mapper.Map<PagedResult<SectorResponseDto>>(entities);
        }
    }
}
