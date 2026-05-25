using Application.DTOs.Exercises;
using Application.Exceptions;
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
    public class ToggleBlockExercises
    {
        private readonly IExercisesRepository _repository;
        private readonly IPeriodsRepository _periodsRepository;
        private readonly IValidator<ExercisesBlockToggleDto> _validator;

        public ToggleBlockExercises(IExercisesRepository repository, IPeriodsRepository periodsRepository, IValidator<ExercisesBlockToggleDto> validator)
        {
            _repository = repository;
            _periodsRepository = periodsRepository;
            _validator = validator;
        }





        public async Task<GlobalResponse> ExecuteAsync(ExercisesBlockToggleDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                                    .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                                    .ToList();

                throw new AppValidationException(errores);
            }

            if (dto.IndBlock == false) 
            {
                var hasActivePeriods = await _periodsRepository.HasActivePeriodsAsync(
                    dto.ExercisesId,
                    dto.BusinessId
                );

                if (hasActivePeriods)
                {
                    throw new BusinessRuleException("No se puede cerrar el ejercicio porque contiene períodos activos. Debe cerrarlos primero.");
                }
            }

            var success = await _repository.PatchBlockStatusAsync(dto.ExercisesId, dto.IndBlock, dto.UsersBy, dto.BusinessId);

            if (!success)
            {
                throw new NotFoundException("Exercises", dto.ExercisesId);
            }

            return new GlobalResponse
            {
                Status = 1,
                Message = dto.IndBlock ? "Ejercicio desbloqueado exitosamente." : "Ejercicio bloqueado exitosamente."
            };
        }
    }
}
