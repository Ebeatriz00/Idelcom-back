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
    public class UpdateExchangeRate
    {
        private readonly IExchangeRateRepository _repository;
        private readonly IValidator<ExchangeRateUpdateDto> _validator;
        private readonly IMapper _mapper;

        public UpdateExchangeRate(IExchangeRateRepository repository, IValidator<ExchangeRateUpdateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(ExchangeRateUpdateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                                .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                                .ToList();
                throw new AppValidationException(errores);
            }

            if (await _repository.ExistsAsync(dto.DateFxrate, dto.BusinessId, dto.ExchangeRateId))
            {
                throw new DuplicateEntryException("Ya existe un tipo de cambio para esa fecha y negocio.");
            }

            var entity = _mapper.Map<Core.Entities.ExchangeRate>(dto);
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
