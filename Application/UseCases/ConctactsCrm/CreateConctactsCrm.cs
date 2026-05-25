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
    public class CreateContactsCrm
    {
        private readonly IContactsCrmRepository _repository;
        private readonly IValidator<ContactsCrmCreateDto> _validator;
        private readonly IMapper _mapper;

        public CreateContactsCrm(
            IContactsCrmRepository repository,
            IValidator<ContactsCrmCreateDto> validator,
            IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(ContactsCrmCreateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                                        .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                                        .ToList();
                throw new AppValidationException(errores);
            }

            var yaExiste = await _repository.ExistsAsync(dto.ContactName, dto.BusinessId);
            if (yaExiste)
                throw new DuplicateEntryException("El contacto ya existe para este negocio.");

            var entity = _mapper.Map<Core.Entities.ContactsCrm>(dto);

            await _repository.AddAsync(entity);

            return new GlobalResponse
            {
                Status = 1,
                Message = "Contacto creado exitosamente.",
            };
        }
    }
}
