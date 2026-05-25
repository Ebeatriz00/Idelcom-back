using Application.DTOs.Activity;
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

namespace Application.UseCases.Activity
{
    public class CreateActivityOppor
    {
        private readonly IActivityRepository _repository;
        private readonly IValidator<ActivityOpporCreateDto> _validator;
        private readonly IMapper _mapper;

        public CreateActivityOppor(IActivityRepository repository, IValidator<ActivityOpporCreateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }
        public async Task<GlobalResponse> ExecuteAsync(ActivityOpporCreateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                           .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                           .ToList();

                throw new AppValidationException(errores);
            }

            var entity = _mapper.Map<Core.Entities.Activity>(dto);
            await _repository.AddActivityOpporAsync(entity);

            return new GlobalResponse
            {
                Status = 1,
                Message = "Actividad agregado exitosamente.",
            };
        }
    }
}
