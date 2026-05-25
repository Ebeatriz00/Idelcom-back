using Application.DTOs.StateTask;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.StateTask
{
    public class GetSelectNormalStateTask 
    {
        private readonly IStateTaskRepository _repository;
        private readonly IMapper _mapper;

        public GetSelectNormalStateTask(IStateTaskRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<List<StateTaskSelectDto>> ExecuteAsync(long businessId)
        {
            var entities = await _repository.GetSelectAsync(businessId);
            return _mapper.Map<List<StateTaskSelectDto>>(entities);
        }
    }
}
