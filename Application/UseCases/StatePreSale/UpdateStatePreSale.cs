using Application.DTOs.StatePreSale;
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


namespace Application.UseCases.StatePreSale
{
    public class UpdateStatePreSale
    {
        private readonly IStatePreSaleRepository _repository;
        private readonly IValidator<StatePreSaleUpdateDto> _validator;
        private readonly IMapper _mapper;

        public UpdateStatePreSale(IStatePreSaleRepository repository, IValidator<StatePreSaleUpdateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(StatePreSaleUpdateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                .ToList();
                throw new AppValidationException(errores);
            }

            if (await _repository.ExistsAsync(dto.StateDesc, dto.BusinessId, dto.StatePreSaleId))
            {
                throw new DuplicateEntryException("El estado de preventa ya existe para este negocio.");
            }

            var entity = _mapper.Map<Core.Entities.StatePreSale>(dto);
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
