using Application.DTOs.MovOper;
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

namespace Application.UseCases.MovOper
{
    public class UpdateMovOper
    {
        private readonly IMovOperRepository _repository;
        private readonly IValidator<MovOperUpdateDto> _validator;
        private readonly IMapper _mapper;

        public UpdateMovOper(IMovOperRepository repository, IValidator<MovOperUpdateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(MovOperUpdateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors.Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage)).ToList();
                throw new AppValidationException(errores);
            }

            if (await _repository.ExistsAsync(dto.Description, dto.BusinessId, dto.MovOperId))
                throw new DuplicateEntryException("El tipo de operación ya existe para este negocio.");

            var entity = _mapper.Map<Core.Entities.MovOper>(dto);
            var updated = await _repository.UpdateAsync(entity);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated ? "Tipo de operación actualizado correctamente." : "Error al actualizar el tipo de operación."
            };
        }
    }
}
