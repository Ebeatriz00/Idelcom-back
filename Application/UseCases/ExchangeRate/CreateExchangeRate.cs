using Application.DTOs.ExchangeRate;
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

namespace Application.UseCases.ExchangeRate
{
    public class CreateExchangeRate
    {
        private readonly IExchangeRateRepository _repository;
        private readonly IValidator<ExchangeRateCreateDto> _validator;
        private readonly IMapper _mapper;

        public CreateExchangeRate(IExchangeRateRepository repository, IValidator<ExchangeRateCreateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(ExchangeRateCreateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                                .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                                .ToList();
                throw new AppValidationException(errores);
            }

            var yaExiste = await _repository.ExistsAsync(dto.DateFxrate, dto.BusinessId);
            if (yaExiste)
                throw new DuplicateEntryException("Ya existe un tipo de cambio para esta fecha y negocio.");

            var entity = _mapper.Map<Core.Entities.ExchangeRate>(dto);
            await _repository.AddAsync(entity);

            return new GlobalResponse
            {
                Status = 1,
                Message = "Tipo de cambio creado exitosamente.",
            };
        }
    }
}
