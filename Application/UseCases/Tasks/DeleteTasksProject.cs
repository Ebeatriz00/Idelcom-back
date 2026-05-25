using Application.DTOs.Tasks;
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


namespace Application.UseCases.Tasks
{
    public class DeleteTasksProject
    {
        private readonly ITasksRepository _repository;
        private readonly IValidator<TasksProjectDeleteDto> _validator;
        private readonly IMapper _mapper;

        public DeleteTasksProject(ITasksRepository repository, IValidator<TasksProjectDeleteDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }
        public async Task<GlobalResponse> ExecuteAsync(TasksProjectDeleteDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                           .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                           .ToList();
                throw new AppValidationException(errores);
            }
            var entity = _mapper.Map<Core.Entities.Tasks>(dto);
            await _repository.DeleteTaskProjectAsync(entity.linkToken, entity.ProjectToken);
            return new GlobalResponse
            {
                Status = 1,
                Message = "Tarea eliminada exitosamente.",
            };
        }
    }
}
