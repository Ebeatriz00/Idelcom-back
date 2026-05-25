using Application.DTOs.CostCenters;
using Application.DTOs.Exercises;
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


namespace Application.UseCases.Exercises
{
    public class UpdateExercises
    {
        private readonly IExercisesRepository _repository;
        private readonly IValidator<ExercisesUpdateDto> _validator;
        private readonly IMapper _mapper;

        public UpdateExercises(IExercisesRepository repository, IValidator<ExercisesUpdateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(ExercisesUpdateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                                .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                                .ToList();

                throw new AppValidationException(errores);
            }

            if (await _repository.ExistsAsync(dto.Description, dto.BusinessId, dto.ExercisesId))
            {
                throw new DuplicateEntryException("El ejercicio ya existe para este negocio.");
            }

            var entity = _mapper.Map<Core.Entities.Exercises>(dto);
            var updated = await _repository.UpdateAsync(entity);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                ? "Ejercicio actualizado correctamente."
                : "No se pudo actualizar el Ejercicio."
            };
        }
    }
}
