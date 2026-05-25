using Application.DTOs.Boxes;
using Application.DTOs.Periods;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Periods
{
    public class GetPeriodsById
    {
        private readonly IPeriodsRepository _repository;
        private readonly IMapper _mapper;

        public GetPeriodsById(IPeriodsRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PeriodsByIdDto> ExecuteAsync(long periodsId)
        {
            var entity = await _repository.GetByIdAsync(periodsId);
            return _mapper.Map<PeriodsByIdDto>(entity);
        }
    }
}
