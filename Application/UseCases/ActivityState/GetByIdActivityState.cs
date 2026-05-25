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
    public class GetByIdActivityState
    {
        private readonly IActivityStateRepository _repository;
        private readonly IMapper _mapper;

        public GetByIdActivityState(IActivityStateRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<ActivityStateResponseDto> ExecuteAsync(long likeToken)
        {
            var entities = await _repository.GetByIdAsync(likeToken);
            return _mapper.Map<ActivityStateResponseDto>(entities);
        }
    }
}
