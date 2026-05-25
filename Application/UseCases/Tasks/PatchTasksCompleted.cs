using Application.DTOs.Tasks;
using Application.Services.RealTime;
using Core.Interfaces;
using Core.Interfaces.Notifications;
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
    public class PatchTasksCompleted
    {
        private readonly ITasksRepository _repository;
        private readonly IValidator<TasksCompletedDto> _validator;
        private readonly INotificationPush _push;

        public PatchTasksCompleted(ITasksRepository repository, IValidator<TasksCompletedDto> validator, INotificationPush push)
        {
            _repository = repository;
            _validator = validator;
            _push = push;
        }

        public async Task<GlobalResponse> ExecuteAsync(TasksCompletedDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                                        .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                                        .ToList();
                throw new AppValidationException(errores);
            }
            var updated = await _repository.PatchTaskCompletedAsync(dto.linkToken, dto.UsersBy, dto.BusinessId);
            await RealtimeEvents.InvalidateBusinessAsync(
                _push,
                dto.BusinessId,
                "opportunities",
                "preSales"
            );
            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                    ? "ha cambiado de estado."
                    : "No se pudo cambiar el estado."
            };
        }
    }
}
