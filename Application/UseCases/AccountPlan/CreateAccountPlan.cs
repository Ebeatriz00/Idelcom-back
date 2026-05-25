using Application.DTOs.AccountPlan;
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

namespace Application.UseCases.AccountPlan
{
    public class CreateAccountPlan
    {
        private readonly IAccountPlanRepository _repository;
        private readonly IValidator<AccountPlanCreateDto> _validator;
        private readonly IMapper _mapper;

        public CreateAccountPlan(IAccountPlanRepository repository, IValidator<AccountPlanCreateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }
        public async Task<GlobalResponse> ExecuteAsync(AccountPlanCreateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                           .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                           .ToList();
                throw new AppValidationException(errores);
            }
            var yaExiste = await _repository.ExistsAsync(dto.AccountCode, dto.BusinessId);
            if (yaExiste)
                throw new DuplicateEntryException("El plan de cuentas ya existe para este negocio.");
            
            var entity = _mapper.Map<Core.Entities.AccountPlan>(dto);
            await _repository.AddAsync(entity);
            return new GlobalResponse
            {
                Status = 1,
                Message = "Plan de cuentas creado exitosamente.",
            };
        }
    }
}
