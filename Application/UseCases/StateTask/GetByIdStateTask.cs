using Application.DTOs.StateTask;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.StateTask
{
    public class GetByIdStateTask
    {
        private readonly IStateTaskRepository _repository;
        private readonly IMapper _mapper;

        public GetByIdStateTask(IStateTaskRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<StateTaskByIdDto> ExecuteAsync(long stateTaskId)
        {
            var entities = await _repository.GetByIdAsync(stateTaskId);
            return _mapper.Map<StateTaskByIdDto>(entities);
        }
    }
}
