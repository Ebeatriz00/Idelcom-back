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
    public class GetSubTasksById
    {
        private readonly ISubTasksRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILinkTokenService _linkTokenService;

        public GetSubTasksById(ISubTasksRepository repository, IMapper mapper, ILinkTokenService linkTokenService)
        {
            _repository = repository;
            _mapper = mapper;
            _linkTokenService = linkTokenService;
        }

        public async Task<SubTasksByIdDto> ExecuteAsync(string linkToken)
        {
            if (!_linkTokenService.ValidateToken(linkToken, out _, out _, out var resourceId) ||
                !long.TryParse(resourceId, out var subTasksId))
            {
                return null;
            }

            var entity = await _repository.GetByIdAsync(subTasksId);
            if (entity == null) return null;

            var dto = _mapper.Map<SubTasksByIdDto>(entity);
            dto.LinkToken = linkToken;

            return dto;
        }
    }
}
