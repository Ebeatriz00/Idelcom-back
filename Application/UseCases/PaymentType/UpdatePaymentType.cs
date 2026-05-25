using Application.DTOs.PaymentType;
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


namespace Application.UseCases.PaymentType
{
    public class UpdatePaymentType
    {
        private readonly IPaymentTypeRepository _repository;
        private readonly IValidator<PaymentTypeUpdateDto> _validator;
        private readonly IMapper _mapper;

        public UpdatePaymentType(IPaymentTypeRepository repository, IValidator<PaymentTypeUpdateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }
        public async Task<GlobalResponse> ExecuteAsync(PaymentTypeUpdateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                .ToList();
                throw new AppValidationException(errores);
            }
            if (await _repository.ExistsAsync(dto.Code,dto.Description, dto.BusinessId, dto.PaymentTypeId))
            {
                throw new DuplicateEntryException("el tipo de pago ya existe para este negocio.");
            }
            var entity = _mapper.Map<Core.Entities.PaymentType>(dto);
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
