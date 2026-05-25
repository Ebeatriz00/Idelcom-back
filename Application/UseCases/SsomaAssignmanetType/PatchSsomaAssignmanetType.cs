using Application.DTOs.Ssoma;
using Application.DTOs.SsomaAssignmanetType;
using Core.Interfaces;
using FluentValidation;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppValidationException = Application.Exceptions.ValidationException;

namespace Application.UseCases.SsomaAssignmanetType
{
    public class PatchSsomaAssignmanetType
    {
        private readonly ISsomaAssignmanetTypeRepository _repository;
        private readonly IValidator<SsomaAssignmanetTypeStatusToggleDto> _validator;

        public PatchSsomaAssignmanetType(ISsomaAssignmanetTypeRepository repository, IValidator<SsomaAssignmanetTypeStatusToggleDto> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<GlobalResponse> ExecuteAsync(SsomaAssignmanetTypeStatusToggleDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                .ToList();
                throw new AppValidationException(errores);
            }

            var updated = await _repository.PatchStatusAsync(dto.SsomaAssignamentTypeId, dto.Status, dto.UsersBy, dto.BusinessId);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
              ? "Estado actualizado correctamente."
              : "No se pudo actualizar el estado."
            };
        }
    }
}
