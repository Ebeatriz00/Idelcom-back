using Application.DTOs.ActivityType;
using Core.Interfaces;
using FluentValidation;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AppValidationException = Application.Exceptions.ValidationException;


namespace Application.UseCases.ActivityType
{
    public class PatchActivityType
    {
        private readonly IActivityTypeRepository _repository;
        private readonly IValidator<ActivityTypeStatusToggleDto> _validator;

        public PatchActivityType(IActivityTypeRepository repository, IValidator<ActivityTypeStatusToggleDto> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<GlobalResponse> ExecuteAsync(ActivityTypeStatusToggleDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                .ToList();
                throw new AppValidationException(errores);
            }

            var updated = await _repository.PatchStatusAsync(dto.LinkToken, dto.Status, dto.UsersBy, dto.BusinessId);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                ? "Prioridad actualizada correctamente."
                : "No se pudo actualizar la prioridad."
            };
        }
    }
}
