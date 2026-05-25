using Application.DTOs.ExchangeRate;
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
    public class PatchExchangeRateStatus
    {
        private readonly IExchangeRateRepository _repository;
        private readonly IValidator<ExchangeRateStatusToggleDto> _validator;

        public PatchExchangeRateStatus(IExchangeRateRepository repository, IValidator<ExchangeRateStatusToggleDto> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<GlobalResponse> ExecuteAsync(ExchangeRateStatusToggleDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                                .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                                .ToList();
                throw new AppValidationException(errores);
            }

            var updated = await _repository.PatchStatusAsync(dto.ExchangeRateId, dto.Status, dto.UsersBy, dto.BusinessId);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                    ? "Estado del tipo de cambio actualizado correctamente."
                    : "No se pudo actualizar el estado del tipo de cambio."
            };
        }
    }
}
