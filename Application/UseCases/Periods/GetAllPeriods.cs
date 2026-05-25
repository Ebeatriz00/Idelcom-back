using Application.DTOs.Periods;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Periods
{
    public class GetAllPeriods
    {
        private readonly IPeriodsRepository _repository;
        private readonly IMapper _mapper;

        public GetAllPeriods(IPeriodsRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PagedResult<PeriodsResponseDto>> ExecuteAsync( long businessId, long exercisesId, string? search, int page, int pageSize, long? usersBy)
        {
            var entities = await _repository.GetAllAsync(businessId, exercisesId, search, page, pageSize, usersBy);
            return _mapper.Map<PagedResult<PeriodsResponseDto>>(entities);
        }
    }
}
