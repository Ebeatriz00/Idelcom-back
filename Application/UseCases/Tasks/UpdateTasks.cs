using Application.DTOs.Tasks;
using Application.Exceptions;
using AutoMapper;
using Core.Interfaces;
using Core.Interfaces.Services;
using FluentValidation;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppValidationException = Application.Exceptions.ValidationException;

namespace Application.UseCases.Tasks
{
    public class UpdateTasks
    {
        private readonly ITasksRepository _repository;
        private readonly IValidator<TasksUpdateDto> _validator;
        private readonly IMapper _mapper;
        private readonly ILinkTokenService _linkTokenService;

        public UpdateTasks(ITasksRepository repository, IValidator<TasksUpdateDto> validator, IMapper mapper, ILinkTokenService linkTokenService)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
            _linkTokenService = linkTokenService;
        }

        public async Task<GlobalResponse> ExecuteAsync(TasksUpdateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                                        .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                                        .ToList();
                throw new AppValidationException(errores);
            }

            if (!_linkTokenService.ValidateToken(dto.LinkToken, out _, out _, out var resourceId) ||
                !long.TryParse(resourceId, out var tasksId))
            {
                return new GlobalResponse { Status = 0, Message = "Token de tarea inválido." };
            }
            var currentTask = await _repository.GetByIdAsync(tasksId);
            if (currentTask == null)
            {
                return new GlobalResponse { Status = 0, Message = "La tarea que intentas editar no existe." };
            }


            bool titleChanged = !string.Equals(currentTask.Title?.Trim(), dto.Title?.Trim(), StringComparison.CurrentCultureIgnoreCase);

            if (titleChanged)
            {

                if (await _repository.ExistsAsync(dto.Title, dto.BusinessId, tasksId))
                {
                    throw new DuplicateEntryException("La tarea ya existe para este negocio.");
                }
            }


            var entity = _mapper.Map<Core.Entities.Tasks>(dto);
            entity.TasksId = tasksId; 

            if (!string.IsNullOrWhiteSpace(dto.OpporToken) &&
                _linkTokenService.ValidateToken(dto.OpporToken, out _, out _, out var opporResId) &&
                long.TryParse(opporResId, out var opporId))
            {
                entity.OpporId = opporId;
            }

            if (!string.IsNullOrWhiteSpace(dto.ProjectToken) &&
                _linkTokenService.ValidateToken(dto.ProjectToken, out _, out _, out var projResId) &&
                long.TryParse(projResId, out var projectId))
            {
                entity.ProjectId = projectId;
            }
            var updated = await _repository.UpdateAsync(entity);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                    ? "Tarea actualizada correctamente."
                    : "Error al actualizar la tarea."
            };
        }
    }
}
