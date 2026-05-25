using Application.DTOs.Tasks;
using Application.Exceptions;
using AutoMapper;
using Core.Interfaces;
using Core.Interfaces.Services;
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
    public class CreateTasks
    {
        private readonly ITasksRepository _repository;
        private readonly IValidator<TasksCreateDto> _validator;
        private readonly IMapper _mapper;
        private readonly ILinkTokenService _linkTokenService;

        public CreateTasks(ITasksRepository repository, IValidator<TasksCreateDto> validator, IMapper mapper, ILinkTokenService linkTokenService)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
            _linkTokenService = linkTokenService;
        }

        public async Task<GlobalResponse> ExecuteAsync(TasksCreateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                                        .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                                        .ToList();
                throw new AppValidationException(errores);
            }

            var yaExiste = await _repository.ExistsAsync(dto.Title, dto.BusinessId);
            if (yaExiste)
                throw new DuplicateEntryException("La tarea ya existe para este negocio.");

            var entity = _mapper.Map<Core.Entities.Tasks>(dto);

            if (!string.IsNullOrWhiteSpace(dto.OpporToken))
            {
                if (_linkTokenService.ValidateToken(dto.OpporToken, out _, out _, out var resourceId) && long.TryParse(resourceId, out var opporIdFromToken))
                {
                    entity.OpporId = opporIdFromToken;
                }
                else if (long.TryParse(dto.OpporToken, out var opporIdDirect))
                {
                    entity.OpporId = opporIdDirect;
                }
            }

            if (!string.IsNullOrWhiteSpace(dto.ProjectToken))
            {
                if (_linkTokenService.ValidateToken(dto.ProjectToken, out _, out _, out var resourceId) && long.TryParse(resourceId, out var projIdFromToken))
                {
                    entity.ProjectId = projIdFromToken;
                }
                else if (long.TryParse(dto.ProjectToken, out var projIdDirect))
                {
                    entity.ProjectId = projIdDirect;
                }
            }

            await _repository.AddAsync(entity);

            return new GlobalResponse
            {
                Status = 1,
                Message = "Tarea creada exitosamente.",
            };
        }
    }
}
