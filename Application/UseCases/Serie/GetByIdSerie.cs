using Application.DTOs.Series;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Series
{
    public class GetSeriesById
    {
        private readonly ISeriesRepository _repository;
        private readonly IMapper _mapper;

        public GetSeriesById(ISeriesRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<SeriesResponseDto> ExecuteAsync(long seriesId)
        {
            var entity = await _repository.GetByIdAsync(seriesId);
            return _mapper.Map<SeriesResponseDto>(entity);
        }
    }
}
