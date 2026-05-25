using Application.DTOs.ContactsCrm;
using Core.Interfaces;
using FluentValidation;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppValidationException = Application.Exceptions.ValidationException;


namespace Application.UseCases.ConctactsCrm
{
    public class PatchContactsCrmStatus
    {
        private readonly IContactsCrmRepository _repository;
        private readonly IValidator<ContactsCrmStatusToggleDto> _validator;

        public PatchContactsCrmStatus(
            IContactsCrmRepository repository,
            IValidator<ContactsCrmStatusToggleDto> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<GlobalResponse> ExecuteAsync(ContactsCrmStatusToggleDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                                        .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                                        .ToList();
                throw new AppValidationException(errores);
            }

            var updated = await _repository.PatchStatusAsync(
                dto.ContactsCrmId, 
                dto.Status,
                dto.UsersBy,
                dto.BusinessId
            );

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                    ? "Estado del contacto actualizado correctamente." 
                    : "No se pudo actualizar el estado del contacto."
            };
        }
    }
}
