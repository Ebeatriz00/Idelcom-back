using Application.DTOs.MovClas;
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

namespace Application.UseCases.MovClas
{
    public class UpdateMovClas
    {
        private readonly IMovClasRepository _repository;
        private readonly IValidator<MovClasUpdateDto> _validator;
        private readonly IMapper _mapper;

        public UpdateMovClas(IMovClasRepository repository, IValidator<MovClasUpdateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(MovClasUpdateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors.Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage)).ToList();
                throw new AppValidationException(errores);
            }

            if (await _repository.ExistsAsync(dto.Description, dto.BusinessId, dto.MovClasId))
                throw new DuplicateEntryException("La clase de movimiento ya existe para este negocio.");

            var entity = _mapper.Map<Core.Entities.MovClas>(dto);
            var updated = await _repository.UpdateAsync(entity);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated ? "Clase de movimiento actualizada correctamente." : "Error al actualizar la clase de movimiento."
            };
        }
    }
}
