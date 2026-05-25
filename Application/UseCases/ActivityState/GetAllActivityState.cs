using Application.DTOs.ActivityState;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.ActivityState
{
    public class GetAllActivityState
    {
        private readonly IActivityStateRepository _repository;
        private readonly IMapper _mapper;

        public GetAllActivityState(IActivityStateRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<PagedResult<ActivityStateResponseDto>> ExecuteAsync(int businessId, string? search, int page, int pageSize)
        {
            var entities = await _repository.GetAllAsync(businessId, search, page, pageSize);
            return _mapper.Map<PagedResult<ActivityStateResponseDto>>(entities);
        }

    }
}
