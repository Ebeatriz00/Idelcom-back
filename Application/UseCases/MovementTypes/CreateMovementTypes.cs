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
    public class CreateMovementTypes
    {
        private readonly IMovementTypesRepository _repository;
        private readonly IValidator<MovementTypesCreateDto> _validator;
        private readonly IMapper _mapper;

        public CreateMovementTypes(IMovementTypesRepository repository, IValidator<MovementTypesCreateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(MovementTypesCreateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                                      .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                                      .ToList();
                throw new AppValidationException(errores);
            }

            if (await _repository.ExistsByDescriptionAsync(dto.Description, dto.BusinessId))
                throw new DuplicateEntryException("La descripción del tipo de movimiento ya existe.");

            if (await _repository.ExistsByCodeAsync(dto.Code, dto.BusinessId))
                throw new DuplicateEntryException("El código del tipo de movimiento ya existe.");

            var entity = _mapper.Map<Core.Entities.MovementTypes>(dto);

            await _repository.AddAsync(entity);

            return new GlobalResponse
            {
                Status = 1,
                Message = "Tipo de movimiento creado exitosamente.",
            };
        }
    }
}
