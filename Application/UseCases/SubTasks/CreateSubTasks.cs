using Application.DTOs.SubTasks;
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
    public class CreateSubTasks
    {
        private readonly ISubTasksRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILinkTokenService _linkTokenService;

        public CreateSubTasks(ISubTasksRepository repository, IMapper mapper, ILinkTokenService linkTokenService)
        {
            _repository = repository;
            _mapper = mapper;
            _linkTokenService = linkTokenService;
        }

        public async Task<bool> ExecuteAsync(SubTasksCreateDto dto)
        {
            if (!_linkTokenService.ValidateToken(dto.TaskToken, out var principal, out _, out var resourceId))
            {
                throw new ArgumentException("El token de la tarea padre es inválido o ha expirado.");
            }

            var entityType = principal?.FindFirst("ent")?.Value;

            if (entityType != "tasks")
            {
                throw new ArgumentException($"El token proporcionado no pertenece a una Tarea. Tipo recibido: {entityType}");
            }

            if (!long.TryParse(resourceId, out var tasksId))
            {
                throw new ArgumentException("El ID de la tarea padre no es válido.");
            }

            var entity = _mapper.Map<Core.Entities.SubTasks>(dto);
            entity.TasksId = tasksId;

            await _repository.AddAsync(entity);
            return true;
        }
    }
}
