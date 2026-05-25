using Application.DTOs.ActivityType;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.ActivityType
{
    public class GetByIdActivityType
    {
        private readonly IActivityTypeRepository _repository;
        private readonly IMapper _mapper;

        public GetByIdActivityType(IActivityTypeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<ActivityTypeResponseDto> ExecuteAsync(string likeToken)
        {
            var entities = await _repository.GetByIdAsync(likeToken);
            return _mapper.Map<ActivityTypeResponseDto>(entities);
        }
    }
}
