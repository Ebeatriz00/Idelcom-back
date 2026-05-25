using Application.DTOs.PriorityState;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.PriorityState
{
    public class GetByIdPriorityState
    {
        private readonly IPriorityStateRepository _repository;
        private readonly IMapper _mapper;

        public GetByIdPriorityState(IPriorityStateRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PriorityStateByIdDto> ExecuteAsync(long priorityStateId)
        {
            var entities = await _repository.GetByIdAsync(priorityStateId);
            return _mapper.Map<PriorityStateByIdDto>(entities);
        }
    }
}
