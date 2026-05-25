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
    public class CreatePaymentType
    {
        private readonly IPaymentTypeRepository _repository;
        private readonly IValidator<PaymentTypeCreateDto> _validator;
        private readonly IMapper _mapper;
        public CreatePaymentType(IPaymentTypeRepository repository, IValidator<PaymentTypeCreateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }
        public async Task<GlobalResponse> ExecuteAsync(PaymentTypeCreateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                           .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                           .ToList();
                throw new AppValidationException(errores);
            }
            var yaExiste = await _repository.ExistsAsync(dto.Code, dto.Description, dto.BusinessId);
            if (yaExiste)
                throw new DuplicateEntryException("El tipo de pago ya existe para este negocio.");
            
            var entity = _mapper.Map<Core.Entities.PaymentType>(dto);
            await _repository.AddAsync(entity);

            return new GlobalResponse
            {
                Status = 1,
                Message = "Tipo de pago creado exitosamente.",
            };
        }
    }
}
