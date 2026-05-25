using Application.DTOs.OpporViability;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.OpporViability
{
    public class GetAllOpporViability
    {
        private readonly IOpporViabilityRepository _repository;
        private readonly IMapper _mapper;

        public GetAllOpporViability(IOpporViabilityRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PagedResult<OpporViabilityResponseDto>> ExecuteAsync(long businessId, string? search, int page, int pageSize, long? usersBy)
        {
            var entities = await _repository.GetAllAsync(businessId, search, page, pageSize, usersBy);
            return _mapper.Map<PagedResult<OpporViabilityResponseDto>>(entities);
        }
    }
}
