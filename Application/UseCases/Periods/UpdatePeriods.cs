using Application.DTOs.Periods;
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


namespace Application.UseCases.Periods
{
    public class UpdatePeriods
    {
        private readonly IPeriodsRepository _repository;
        private readonly IValidator<PeriodsUpdateDto> _validator;
        private readonly IMapper _mapper;

        public UpdatePeriods(IPeriodsRepository repository, IValidator<PeriodsUpdateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(PeriodsUpdateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                                        .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                                        .ToList();
                throw new AppValidationException(errores);
            }

            if (await _repository.ExistsAsync(dto.Description, dto.ExercisesId, dto.BusinessId, dto.PeriodsId))
            {
                throw new DuplicateEntryException("El período ya existe para este ejercicio.");
            }

            var entity = _mapper.Map<Core.Entities.Periods>(dto);
            var updated = await _repository.UpdateAsync(entity);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                    ? "Período actualizado correctamente."
                    : "Error al actualizar el período."
            };
        }
    }
}
