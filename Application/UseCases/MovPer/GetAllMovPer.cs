using Application.DTOs.MovPer;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.MovPer
{
    public class GetAllMovPer
    {
        private readonly IMovPerRepository _repository;
        private readonly IMapper _mapper;

        public GetAllMovPer(IMovPerRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PagedResult<MovPerResponseDto>> ExecuteAsync(long businessId, string? search, int page, int pageSize)
        {
            var entities = await _repository.GetAllAsync(businessId, search, page, pageSize);
            return _mapper.Map<PagedResult<MovPerResponseDto>>(entities);
        }
    }
}
