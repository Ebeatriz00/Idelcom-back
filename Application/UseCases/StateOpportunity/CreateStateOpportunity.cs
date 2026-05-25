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
    public class CreateStateOpportunity
    {
        private readonly IStateOpportunityRepository _repository;
        private readonly IValidator<StateOpportunityCreateDto> _validator;
        private readonly IMapper _mapper;

        public CreateStateOpportunity(IStateOpportunityRepository repository, IValidator<StateOpportunityCreateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(StateOpportunityCreateDto dto)
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
                throw new DuplicateEntryException("El estado de oportunidades ya existe para este negocio.");

            var entity = _mapper.Map<Core.Entities.StateOpportunity>(dto);
            await _repository.AddAsync(entity);

            return new GlobalResponse
            {
                Status = 1,
                Message = "Estado de oportunidades creado exitosamente.",
            };
        }
    }
}
