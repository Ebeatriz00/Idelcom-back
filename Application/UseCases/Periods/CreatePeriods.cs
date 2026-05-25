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
    public class CreatePeriods
    {
        private readonly IPeriodsRepository _repository;
        private readonly IValidator<PeriodsCreateDto> _validator;
        private readonly IMapper _mapper;

        public CreatePeriods(IPeriodsRepository repository, IValidator<PeriodsCreateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(PeriodsCreateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                                        .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                                        .ToList();
                throw new AppValidationException(errores);
            }

            var yaExiste = await _repository.ExistsAsync(dto.Description, dto.ExercisesId, dto.BusinessId);
            if (yaExiste)
                throw new DuplicateEntryException("El período ya existe para este ejercicio.");

            var entity = _mapper.Map<Core.Entities.Periods>(dto);

            await _repository.AddAsync(entity);

            return new GlobalResponse
            {
                Status = 1,
                Message = "Período creado exitosamente.",
            };
        }
    }
}
