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
    public class DeleteActivityProject
    {
        private readonly IActivityRepository _repository;
        private readonly IValidator<ActivityDeleteProjectDto> _validator;
        private readonly IMapper _mapper;

        public DeleteActivityProject(IActivityRepository repository, IValidator<ActivityDeleteProjectDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }
        public async Task<GlobalResponse> ExecuteAsync(ActivityDeleteProjectDto dto)
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
            await _repository.DeleteActivityProjectAsync(entity.LinkToken, entity.ProjectToken);

            return new GlobalResponse
            {
                Status = 1,
                Message = "Actividad eliminada exitosamente.",
            };
        }
    }
}
