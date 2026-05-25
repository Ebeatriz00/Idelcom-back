using Application.DTOs.SubTasks.Application.DTOs.SubTasks;
using AutoMapper;
using Core.Interfaces;
using Core.Interfaces.Services;
using FluentValidation;
using Org.BouncyCastle.Tsp;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppValidationException = Application.Exceptions.ValidationException;


namespace Application.UseCases.SubTasks
{
    public class UpdateSubTasks
    {
        private readonly ISubTasksRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILinkTokenService _linkTokenService;

        public UpdateSubTasks(ISubTasksRepository repository, IMapper mapper, ILinkTokenService linkTokenService)
        {
            _repository = repository;
            _mapper = mapper;
            _linkTokenService = linkTokenService;
        }

        public async Task<bool> ExecuteAsync(SubTasksUpdateDto dto)
        {
            if (!_linkTokenService.ValidateToken(dto.LinkToken, out var principal, out _, out var resourceId))
            {
                throw new ArgumentException("El token de la sub-tarea es inválido o ha expirado.");
            }

            var entityType = principal?.FindFirst("ent")?.Value;

            if (entityType != "subtask")
            {
                throw new ArgumentException($"Token incorrecto. Se esperaba un token de 'subtask', pero se recibió uno de tipo '{entityType}'.");
            }

            if (!long.TryParse(resourceId, out var subTasksId))
            {
                throw new ArgumentException("El ID contenido en el token no es válido.");
            }

            var entity = _mapper.Map<Core.Entities.SubTasks>(dto);
            entity.SubTasksId = subTasksId;

            return await _repository.UpdateAsync(entity);
        }
    }
}
