using Application.DTOs.PaymentType;
using Application.DTOs.StateOpportunity;
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


namespace Application.UseCases.StateOpportunity
{
    public class UpdateStateOpportunity
    {
        private readonly IStateOpportunityRepository _repository;
        private readonly IValidator<StateOpportunityUpdateDto> _validator;
        private readonly IMapper _mapper;

        public UpdateStateOpportunity(IStateOpportunityRepository repository, IValidator<StateOpportunityUpdateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(StateOpportunityUpdateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                .ToList();
                throw new AppValidationException(errores);
            }
            if (await _repository.ExistsAsync(dto.StateDesc, dto.BusinessId, dto.StateOpportunityId))
            {
                throw new DuplicateEntryException("el estado de oportunidades ya existe para este negocio.");
            }
            var entity = _mapper.Map<Core.Entities.StateOpportunity>(dto);
            var updated = await _repository.UpdateAsync(entity);
            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                ? "Actualizado correctamente."
                : "Error al actualizar."
            };
        }
    }
}
