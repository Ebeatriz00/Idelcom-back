using Application.DTOs.Activity;
using Core.Interfaces;
using FluentValidation;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AppValidationException = Application.Exceptions.ValidationException;

namespace Application.UseCases.Activity
{
    public class PatchChangeActivityPriorityOppor
    {
        private readonly IActivityRepository _repository;
        private readonly IValidator<ActivityPriorityOpporDto> _validator;
        public PatchChangeActivityPriorityOppor(IActivityRepository repository, IValidator<ActivityPriorityOpporDto> validator)
        {
            _repository = repository;
            _validator = validator;
        }
        public async Task<GlobalResponse> ExecuteAsync(ActivityPriorityOpporDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                                        .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                                        .ToList();
                throw new AppValidationException(errores);
            }
            var updated = await _repository.PatchActivityOpporPriorityStateAsync(dto.LinkToken, dto.Status, dto.UsersBy, dto.BusinessId);
            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                    ? "ha cambiado de prioridad."
                    : "No se pudo cambiar la prioridad."
            };
        }
    }
}
