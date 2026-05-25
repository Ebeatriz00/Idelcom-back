using Application.DTOs.Currency;
using Application.DTOs.DocumentType;
using AppValidationException = Application.Exceptions.ValidationException;
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
    public class PatchCurrencyStatus
    {

        private readonly ICurrencyRepository _repository;
        private readonly IValidator<CurrencyStatusToggleDto> _validator;

        public PatchCurrencyStatus(ICurrencyRepository repository, IValidator<CurrencyStatusToggleDto> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<GlobalResponse> ExecuteAsync(CurrencyStatusToggleDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                .ToList();

                throw new AppValidationException(errores);

            }

            var updated = await _repository.PatchStatusAsync(dto.CurrencyId, dto.Status, dto.UsersBy, dto.BusinessId);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                ? "Estado de la moneda actualizado correctamente."
                : "No se pudo actualizar el estado de la moneda."
            };
        }
    }
}
