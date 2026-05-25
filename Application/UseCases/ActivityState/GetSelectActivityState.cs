using Application.DTOs.ActivityState;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.ActivityState
{
    public class GetSelectActivityState
    {
        private readonly IActivityStateRepository _repository;
        private readonly IMapper _mapper;

        public GetSelectActivityState(IActivityStateRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<List<ActivityStateSelectDto>> ExecuteAsync(long businessId)
        {
            var entities = await _repository.GetSelectAsync(businessId);
            return _mapper.Map<List<ActivityStateSelectDto>>(entities);
        }
    }
}
