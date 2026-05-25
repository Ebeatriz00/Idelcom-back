using Application.DTOs.Ssoma;
using Application.DTOs.SsomaAssignmanetType;
using Application.Exceptions;
using AutoMapper;
using Core.Entities;
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
    public class UpdateSsomaAssignmanetType
    {
        private readonly ISsomaAssignmanetTypeRepository _repository;
        private readonly IValidator<SsomaAsiggnmanetTypeUpdateDto> _validator;
        private readonly IMapper _mapper;

        public UpdateSsomaAssignmanetType(ISsomaAssignmanetTypeRepository repository, IValidator<SsomaAsiggnmanetTypeUpdateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(SsomaAsiggnmanetTypeUpdateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                .ToList();
                throw new AppValidationException(errores);
            }
            if (await _repository.ExistsAsync(dto.SsomaAssignamentName, dto.BusinessId, dto.SsomaAssignamentTypeId))
            {
                throw new DuplicateEntryException("El tipo de asignamiento ya existe para este negocio.");
            }
            var entity = _mapper.Map<Core.Entities.SsomaAssignmanetType>(dto);
            var updated = await _repository.UpdateAsync(entity);
            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                ? "Actualizado correctamente."
                : "Error al actualizar."
            };

        }
    }
}
