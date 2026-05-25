using Application.DTOs.PriorityState;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.PriorityState
{
    public class GetAllPriorityState
    {
        private readonly IPriorityStateRepository _repository;
        private readonly IMapper _mapper;

        public GetAllPriorityState(IPriorityStateRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PagedResult<PriorityStateResponseDto>> ExecuteAsync(long businessId, string? search, int page, int pageSize)
        {
            var entities = await _repository.GetAllAsync(businessId, search, page, pageSize);
            return _mapper.Map<PagedResult<PriorityStateResponseDto>>(entities);
        }
    }
}
