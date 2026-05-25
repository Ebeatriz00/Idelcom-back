using Application.DTOs.ContactType;
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

namespace Application.UseCases.ContactType
{
    public class UpdateContactType
    {
        private readonly IContactTypeRepository _repository;
        private readonly IValidator<ContactTypeUpdateDto> _validator;
        private readonly IMapper _mapper;

        public UpdateContactType(IContactTypeRepository repository, IValidator<ContactTypeUpdateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(ContactTypeUpdateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                                      .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                                      .ToList();
                throw new AppValidationException(errores);
            }

            if (await _repository.ExistsAsync(dto.Description, dto.BusinessId, dto.ContactTypeId))
            {
                throw new DuplicateEntryException("El tipo de contacto ya existe para este negocio.");
            }

            var entity = _mapper.Map<Core.Entities.ContactType>(dto);
            var updated = await _repository.UpdateAsync(entity);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                    ? "Tipo de contacto actualizado correctamente."
                    : "Error al actualizar el tipo de contacto."
            };
        }
    }
}
