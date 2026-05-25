using Application.DTOs.MovementTypes;
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

namespace Application.UseCases.MovementTypes
{
    public class UpdateMovementTypes
    {
        private readonly IMovementTypesRepository _repository;
        private readonly IValidator<MovementTypesUpdateDto> _validator;
        private readonly IMapper _mapper;

        public UpdateMovementTypes(IMovementTypesRepository repository, IValidator<MovementTypesUpdateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(MovementTypesUpdateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                                      .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                                      .ToList();
                throw new AppValidationException(errores);
            }

            if (await _repository.ExistsByDescriptionAsync(dto.Description, dto.BusinessId, dto.MovementTypesId))
                throw new DuplicateEntryException("La descripción del tipo de movimiento ya existe.");

            if (await _repository.ExistsByCodeAsync(dto.Code, dto.BusinessId, dto.MovementTypesId))
                throw new DuplicateEntryException("El código del tipo de movimiento ya existe.");

            var entity = _mapper.Map<Core.Entities.MovementTypes>(dto);
            var updated = await _repository.UpdateAsync(entity);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                    ? "Tipo de movimiento actualizado correctamente."
                    : "Error al actualizar el tipo de movimiento."
            };
        }
    }
}
