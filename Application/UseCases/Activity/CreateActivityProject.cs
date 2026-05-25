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
    public class CreateActivityProject
    {
        private readonly IActivityRepository _repository;
        private readonly IMapper _mapper;

        public CreateActivityProject(IActivityRepository repository,  IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<GlobalResponse> ExecuteAsync(ActivityProjectCreateDto dto)
        {
            var entity = _mapper.Map<Core.Entities.Activity>(dto);
            await _repository.AddActivityProjectAsync(entity);

            return new GlobalResponse
            {
                Status = 1,
                Message = "Actividad agregado exitosamente.",
            };
        }
    }
}
