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
    public class GetSelectNormalPriorityState
    {
        private readonly IPriorityStateRepository _repository;
        private readonly IMapper _mapper;

        public GetSelectNormalPriorityState(IPriorityStateRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<List<PriorityStateSelectDto>> ExecuteAsync(long businessId)
        {
            var entities = await _repository.GetSelectAsync(businessId);
            return _mapper.Map<List<PriorityStateSelectDto>>(entities);
        }
    }
}
