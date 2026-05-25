using Application.DTOs.Tasks;
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
    public class PatchTasksStatus
    {
        private readonly ITasksRepository _repository;
        private readonly IValidator<TasksStatusToggleDto> _validator;
        private readonly ILinkTokenService _linkTokenService;

        public PatchTasksStatus(ITasksRepository repository, IValidator<TasksStatusToggleDto> validator, ILinkTokenService linkTokenService)
        {
            _repository = repository;
            _validator = validator;
            _linkTokenService = linkTokenService;
        }

        public async Task<GlobalResponse> ExecuteAsync(TasksStatusToggleDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                                        .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                                        .ToList();
                throw new AppValidationException(errores);
            }

            // Decodificar LinkToken
            if (!_linkTokenService.ValidateToken(dto.LinkToken, out _, out _, out var resourceId) ||
                !long.TryParse(resourceId, out var tasksId))
            {
                return new GlobalResponse { Status = 0, Message = "Token inválido." };
            }

            var updated = await _repository.PatchStatusAsync(tasksId, dto.Status, dto.UsersBy, dto.BusinessId);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                    ? "Estado de la tarea actualizado correctamente."
                    : "No se pudo actualizar el estado de la tarea."
            };
        }
    }
}
