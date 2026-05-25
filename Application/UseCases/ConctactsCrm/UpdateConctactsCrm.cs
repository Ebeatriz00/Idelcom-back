using Application.DTOs.ContactsCrm;
using Application.Exceptions;
using AutoMapper;
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
    public class UpdateContactsCrm
    {
        private readonly IContactsCrmRepository _repository;
        private readonly IValidator<ContactsCrmUpdateDto> _validator;
        private readonly IMapper _mapper;

        public UpdateContactsCrm(
            IContactsCrmRepository repository,
            IValidator<ContactsCrmUpdateDto> validator,
            IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(ContactsCrmUpdateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                                        .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                                        .ToList();
                throw new AppValidationException(errores);
            }

            if (await _repository.ExistsAsync(dto.ContactName, dto.BusinessId, dto.ContactsCrmId))
            {
                throw new DuplicateEntryException("El contacto ya existe para este negocio.");
            }

            var entity = _mapper.Map<Core.Entities.ContactsCrm>(dto);
            var updated = await _repository.UpdateAsync(entity);
           
            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                    ? "Contacto actualizado correctamente."
                    : "Error al actualizar el contacto."
            };
        }
    }
}
