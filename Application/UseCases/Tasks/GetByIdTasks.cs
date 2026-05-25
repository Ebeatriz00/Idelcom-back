using Application.DTOs.Tasks;
using AutoMapper;
using Core.Interfaces;
using Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Tasks
{
    public class GetTasksById
    {
        private readonly ITasksRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILinkTokenService _linkTokenService;

        public GetTasksById(ITasksRepository repository, IMapper mapper, ILinkTokenService linkTokenService)
        {
            _repository = repository;
            _mapper = mapper;
            _linkTokenService = linkTokenService;
        }

        // Cambio: Recibe string linkToken
        public async Task<TasksByIdDto> ExecuteAsync(string linkToken)
        {
            if (!_linkTokenService.ValidateToken(linkToken, out _, out _, out var resourceId) ||
                !long.TryParse(resourceId, out var tasksId))
            {
                return null;
            }

            var entity = await _repository.GetByIdAsync(tasksId);
            return _mapper.Map<TasksByIdDto>(entity);
        }
    }
}
