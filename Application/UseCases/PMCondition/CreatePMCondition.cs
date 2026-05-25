using Application.DTOs.PMCondition;
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


namespace Application.UseCases.PMCondition
{
    public class CreatePMCondition
    {
        private readonly IPMConditionRepository _repository;
        private readonly IValidator<PMConditionCreateDto> _validator;
        private readonly IMapper _mapper;

        public CreatePMCondition(IPMConditionRepository repository, IValidator<PMConditionCreateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(PMConditionCreateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors.Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage)).ToList();
                throw new AppValidationException(errores);
            }

            if (await _repository.ExistsAsync(dto.Description, dto.BussinessId))
                throw new DuplicateEntryException("La condición de pago ya existe para este negocio.");

            var entity = _mapper.Map<Core.Entities.PMCondition>(dto);
            await _repository.AddAsync(entity);

            return new GlobalResponse
            {
                Status = 1,
                Message = "Condición de pago creada exitosamente."
            };
        }
    }
}
