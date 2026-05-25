using Application.DTOs.Currency;
using Application.DTOs.DocumentType;
using Application.Exceptions;
using AutoMapper;
using Core.Interfaces;
using FluentValidation;
using SharedKernel;
using SharedKernel.Constants;
using SharedKernel.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppValidationException = Application.Exceptions.ValidationException;
namespace Application.UseCases.Currency
{
    public class CreateCurrency
    {
        private readonly ICurrencyRepository _repository;
        private readonly IValidator<CurrencyCreateDto> _validator;
        private readonly IMapper _mapper;

        public CreateCurrency(ICurrencyRepository repository, IValidator<CurrencyCreateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(CurrencyCreateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                           .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                           .ToList();

                throw new AppValidationException(errores);
            }

            var yaExiste = await _repository.ExistsAsync(dto.Description, dto.CodeSunat, dto.BusinessId);
            if (yaExiste)
                throw new DuplicateEntryException("La moneda ya existe para este negocio.");


            var entity = _mapper.Map<Core.Entities.Currency>(dto);
            await _repository.AddAsync(entity);

            return new GlobalResponse
            {
                Status = 1,
                Message = "Moneda creada exitosamente.",
            };
        }

    }

}
