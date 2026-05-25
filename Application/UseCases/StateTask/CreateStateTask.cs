using Application.DTOs.StateTask;
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


namespace Application.UseCases.StateTask
{
    public class CreateStateTask
    {
        private readonly IStateTaskRepository _repository;
        private readonly IValidator<StateTaskCreateDto> _validator;
        private readonly IMapper _mapper;

        public CreateStateTask(IStateTaskRepository repository, IValidator<StateTaskCreateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(StateTaskCreateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                                .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                                .ToList();
                throw new AppValidationException(errores);
            }
            var yaExiste = await _repository.ExistsAsync(dto.StateDesc, dto.BusinessId);
            if (yaExiste)
                throw new DuplicateEntryException("El estado de tareas ya existe para este negocio.");

            var entity = _mapper.Map<Core.Entities.StateTask>(dto);
            await _repository.AddAsync(entity);

            return new GlobalResponse
            {
                Status = 1,
                Message = "Estado de tareas creado exitosamente.",
            };
        }
    }
}
