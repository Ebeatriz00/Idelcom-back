using Application.DTOs.MovVis;
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

namespace Application.UseCases.MovVis
{
    public class UpdateMovVis
    {
        private readonly IMovVisRepository _repository;
        private readonly IValidator<MovVisUpdateDto> _validator;
        private readonly IMapper _mapper;

        public UpdateMovVis(IMovVisRepository repository, IValidator<MovVisUpdateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(MovVisUpdateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors.Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage)).ToList();
                throw new AppValidationException(errores);
            }

            if (await _repository.ExistsAsync(dto.Description, dto.BusinessId, dto.MovVisId))
                throw new DuplicateEntryException("La visibilidad de movimiento ya existe para este negocio.");

            var entity = _mapper.Map<Core.Entities.MovVis>(dto);
            var updated = await _repository.UpdateAsync(entity);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated 
                ? "Visibilidad de movimiento actualizada correctamente." 
                : "Error al actualizar la visibilidad de movimiento."
            };
        }
    }
}
