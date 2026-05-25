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
    public class UpdatePriorityState
    {
        private readonly IPriorityStateRepository _repository;
        private readonly IValidator<PriorityStateUpdateDto> _validator;
        private readonly IMapper _mapper;

        public UpdatePriorityState(IPriorityStateRepository repository, IValidator<PriorityStateUpdateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(PriorityStateUpdateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                .ToList();
                throw new AppValidationException(errores);
            }

            if (await _repository.ExistsAsync(dto.PriorityDesc, dto.BusinessId, dto.PriorityStateId))
            {
                throw new DuplicateEntryException("La prioridad ya existe para este negocio.");
            }

            var entity = _mapper.Map<Core.Entities.PriorityState>(dto);
            var updated = await _repository.UpdateAsync(entity);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                ? "Prioridad actualizada correctamente."
                : "Error al actualizar la prioridad."
            };
        }
    }
}
