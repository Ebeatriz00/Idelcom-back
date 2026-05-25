using Application.DTOs.ContactType;
using Core.Interfaces;
using FluentValidation;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppValidationException = Application.Exceptions.ValidationException;

namespace Application.UseCases.ContactType
{
    public class PatchContactTypeStatus
    {
        private readonly IContactTypeRepository _repository;
        private readonly IValidator<ContactTypeStatusToggleDto> _validator;

        public PatchContactTypeStatus(IContactTypeRepository repository, IValidator<ContactTypeStatusToggleDto> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<GlobalResponse> ExecuteAsync(ContactTypeStatusToggleDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                                      .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                                      .ToList();
                throw new AppValidationException(errores);
            }

            var updated = await _repository.PatchStatusAsync(dto.ContactTypeId, dto.Status, dto.UsersBy, dto.BusinessId);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                    ? "Estado del tipo de contacto actualizado correctamente."
                    : "No se pudo actualizar el estado del tipo de contacto."
            };
        }
    }
}
