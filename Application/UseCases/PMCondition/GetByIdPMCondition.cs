using Application.DTOs.PMCondition;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.PMCondition
{
    public class GetPMConditionById
    {
        private readonly IPMConditionRepository _repository;
        private readonly IMapper _mapper;

        public GetPMConditionById(IPMConditionRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PMConditionResponseDto> ExecuteAsync(long pmConditionId)
        {
            var entity = await _repository.GetByIdAsync(pmConditionId);
            return _mapper.Map<PMConditionResponseDto>(entity);
        }
    }
}
