using Application.DTOs.MovSunat;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.MovSunat
{
    public class GetAllMovSunat
    {
        private readonly IMovSunatRepository _repository;
        private readonly IMapper _mapper;

        public GetAllMovSunat(IMovSunatRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PagedResult<MovSunatResponseDto>> ExecuteAsync(long businessId, string? search, int page, int pageSize)
        {
            var entities = await _repository.GetAllAsync(businessId, search, page, pageSize);
            return _mapper.Map<PagedResult<MovSunatResponseDto>>(entities);
        }
    }
}
