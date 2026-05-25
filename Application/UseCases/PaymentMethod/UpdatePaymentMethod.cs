using Application.DTOs.PaymentMethod;
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


namespace Application.UseCases.PaymentMethod
{
    public class UpdatePaymentMethod
    {
        private readonly IPaymentMethodRepository _repository;
        private readonly IValidator<PaymentMethodUpdateDto> _validator;
        private readonly IMapper _mapper;

        public UpdatePaymentMethod(
            IPaymentMethodRepository repository,
            IValidator<PaymentMethodUpdateDto> validator,
            IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(PaymentMethodUpdateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                                        .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                                        .ToList();
                throw new AppValidationException(errores);
            }

            if (await _repository.ExistsAsync(dto.Description, dto.BusinessId, dto.PaymentMethodId))
            {
                throw new DuplicateEntryException("El método de pago ya existe para este negocio.");
            }

            var entity = _mapper.Map<Core.Entities.PaymentMethod>(dto);
            var updated = await _repository.UpdateAsync(entity);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                    ? "Método de pago actualizado correctamente."
                    : "Error al actualizar el método de pago."
            };
        }
    }
}
