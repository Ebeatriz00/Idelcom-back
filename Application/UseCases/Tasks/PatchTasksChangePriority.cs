using Application.DTOs.Tasks;
using Core.Interfaces;
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
    public class PatchTasksPriorityState
    {
        private readonly ITasksRepository _repository;
        private readonly IValidator<TaskChangePriorityStateDto> _validator;
        public PatchTasksPriorityState(ITasksRepository repository, IValidator<TaskChangePriorityStateDto> validator)
        {
            _repository = repository;
            _validator = validator;
        }
        public async Task<GlobalResponse> ExecuteAsync(TaskChangePriorityStateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                                        .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                                        .ToList();
                throw new AppValidationException(errores);
            }
            var updated = await _repository.PatchTaskPriorityStateAsync(dto.linkToken, dto.Status, dto.UsersBy, dto.BusinessId);
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
