using Application.DTOs.MovPer;
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

namespace Application.UseCases.MovPer
{
    public class UpdateMovPer
    {
        private readonly IMovPerRepository _repository;
        private readonly IValidator<MovPerUpdateDto> _validator;
        private readonly IMapper _mapper;

        public UpdateMovPer(IMovPerRepository repository, IValidator<MovPerUpdateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(MovPerUpdateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors.Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage)).ToList();
                throw new AppValidationException(errores);
            }

            if (await _repository.ExistsAsync(dto.Description, dto.BusinessId, dto.MovPerId))
                throw new DuplicateEntryException("El periodo de movimiento ya existe para este negocio.");

            var entity = _mapper.Map<Core.Entities.MovPer>(dto);
            var updated = await _repository.UpdateAsync(entity);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated 
                ? "Periodo de movimiento actualizado correctamente." 
                : "Error al actualizar el periodo de movimiento."
            };
        }
    }
}
