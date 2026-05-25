using Application.DTOs.CostCenters;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.CostCenters
{
    public class GetByIdCostCenters
    {
        private readonly ICostCentersRepository _repository;
        private readonly IMapper _mapper;

        public GetByIdCostCenters(ICostCentersRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<CostCentersResponseDto> ExecuteAsync(long costCentersId)
        {
            var entity = await _repository.GetByIdAsync(costCentersId);

            return _mapper.Map<CostCentersResponseDto>(entity);
        }
    }
}
