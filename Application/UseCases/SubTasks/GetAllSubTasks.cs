using Application.DTOs.SubTasks;
using AutoMapper;
using Core.Interfaces;
using Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.SubTasks
{
    public class GetAllSubTasks
    {
        private readonly ISubTasksRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILinkTokenService _linkTokenService;

        public GetAllSubTasks(ISubTasksRepository repository, IMapper mapper, ILinkTokenService linkTokenService)
        {
            _repository = repository;
            _mapper = mapper;
            _linkTokenService = linkTokenService;
        }

        public async Task<List<SubTasksResponseDto>> ExecuteAsync(long businessId, string? taskToken)
        {
            long? tasksId = null;  

            if (!string.IsNullOrEmpty(taskToken))
            {
                if (!_linkTokenService.ValidateToken(taskToken, out _, out _, out var resourceId) ||
                    !long.TryParse(resourceId, out var parsedId))
                {
                    return new List<SubTasksResponseDto>();
                }
                tasksId = parsedId; 
            }

            var entities = await _repository.GetAllByTaskAsync(businessId, tasksId);

            return _mapper.Map<List<SubTasksResponseDto>>(entities);
        }
    }
}
