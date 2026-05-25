using Application.DTOs.PaymentMethod;
using Core.Interfaces;
using FluentValidation;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppValidationException = Application.Exceptions.ValidationException;


namespace Application.UseCases.PaymentMethod
{
    public class PatchPaymentMethodStatus
    {
        private readonly IPaymentMethodRepository _repository;
        private readonly IValidator<PaymentMethodStatusToggleDto> _validator;

        public PatchPaymentMethodStatus(
            IPaymentMethodRepository repository,
            IValidator<PaymentMethodStatusToggleDto> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<GlobalResponse> ExecuteAsync(PaymentMethodStatusToggleDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                                        .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                                        .ToList();
                throw new AppValidationException(errores);
            }

            var updated = await _repository.PatchStatusAsync(
                dto.PaymentMethodId,
                dto.Status,
                dto.UsersBy,
                dto.BusinessId
            );

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                    ? "Estado del método de pago actualizado correctamente."
                    : "No se pudo actualizar el estado del método de pago."
            };
        }
    }
}
