using Application.DTOs.Currency;
using AppValidationException = Application.Exceptions.ValidationException;
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

namespace Application.UseCases.Currency
{
    public class UpdateCurrency
    {
        private readonly ICurrencyRepository _repository;
        private readonly IValidator<CurrencyUpdateDto> _validator;
        private readonly IMapper _mapper;

        public UpdateCurrency(ICurrencyRepository repository, IValidator<CurrencyUpdateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }
        public async Task<GlobalResponse> ExecuteAsync(CurrencyUpdateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                .ToList();

                throw new AppValidationException(errores);

            }
            if (await _repository.ExistsAsync(dto.Description, dto.CodeSunat, dto.BusinessId, dto.CurrencyId))
            {
                throw new DuplicateEntryException("la moneda ya existe para este negocio.");

            }


            var entity = _mapper.Map<Core.Entities.Currency>(dto);

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
