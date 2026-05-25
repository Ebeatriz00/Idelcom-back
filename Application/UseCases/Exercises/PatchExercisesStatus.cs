using Application.DTOs.ConceptGroups;
using Application.DTOs.Exercises;
using Core.Interfaces;
using FluentValidation;
using Infrastructure.Repositories;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppValidationException = Application.Exceptions.ValidationException;


namespace Application.UseCases.Exercises
{
    public class PatchExercisesStatus
    {
        private readonly IExercisesRepository _repository;
        private readonly IValidator<ExercisesStatusToggleDto> _validator;

        public PatchExercisesStatus(IExercisesRepository repository, IValidator<ExercisesStatusToggleDto> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<GlobalResponse> ExecuteAsync(ExercisesStatusToggleDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                                      .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                                      .ToList();
                throw new AppValidationException(errores);
            }

            var updated = await _repository.PatchStatusAsync(dto.ExercisesId, dto.Status, dto.UsersBy, dto.BusinessId);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                    ? "Estado del ejercicio actualizado correctamente."
                    : "No se pudo actualizar el estado del ejercicio."
            };
        }
    }
}
