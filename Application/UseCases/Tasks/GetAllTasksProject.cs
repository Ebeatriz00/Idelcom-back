using Application.DTOs.Tasks;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Tasks
{
    public class GetAllTasksProject
    {
        private readonly ITasksRepository _repository;
        private readonly IMapper _mapper;

        public GetAllTasksProject(ITasksRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PagedResult<TasksProjectResponseDto>> ExecuteAsync(long businessId, string? search, int page, int pageSize, long? opporId)
        {
            var entities = await _repository.GetAllProjectsAsync(businessId, search, page, pageSize, opporId);
            return _mapper.Map<PagedResult<TasksProjectResponseDto>>(entities);
        }
    }
}
