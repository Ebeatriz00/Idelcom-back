using Application.DTOs.PriorityState;
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


namespace Application.UseCases.PriorityState
{
    public class CreatePriorityState
    {
        private readonly IPriorityStateRepository _repository;
        private readonly IValidator<PriorityStateCreateDto> _validator;
        private readonly IMapper _mapper;

        public CreatePriorityState(IPriorityStateRepository repository, IValidator<PriorityStateCreateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(PriorityStateCreateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                                 .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                                 .ToList();
                throw new AppValidationException(errores);
            }

            var yaExiste = await _repository.ExistsAsync(dto.PriorityDesc, dto.BusinessId);
            if (yaExiste)
                throw new DuplicateEntryException("La prioridad ya existe para este negocio.");

            var entity = _mapper.Map<Core.Entities.PriorityState>(dto);
            await _repository.AddAsync(entity);

            return new GlobalResponse
            {
                Status = 1,
                Message = "Prioridad creada exitosamente.",
            };
        }
    }
}
