using Application.DTOs.Series;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Series
{
   

    public class GetAllSeries
    {
        private readonly ISeriesRepository _repository;
        private readonly IMapper _mapper;

        public GetAllSeries(ISeriesRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PagedResult<SeriesResponseDto>> ExecuteAsync(long businessId, string? search, int page, int pageSize, long? usersBy)
        {
            var entities = await _repository.GetAllAsync(businessId,search, page, pageSize, usersBy);
            return _mapper.Map<PagedResult<SeriesResponseDto>>(entities);
        }
    }
}
