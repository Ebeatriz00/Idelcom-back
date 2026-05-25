using Application.DTOs.TaxAffType;
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


namespace Application.UseCases.TaxAffType
{
    public class CreateTaxAffType
    {
        private readonly ITaxAffTypeRepository _repository;
        private readonly IValidator<TaxAffTypeCreateDto> _validator;
        private readonly IMapper _mapper;
        public CreateTaxAffType(ITaxAffTypeRepository repository, IValidator<TaxAffTypeCreateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }
        public async Task<GlobalResponse> ExecuteAsync(TaxAffTypeCreateDto dto)
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
                throw new DuplicateEntryException("El tipo de afectación tributaria ya existe para este negocio.");
            
            var entity = _mapper.Map<Core.Entities.TaxAffType>(dto);
            await _repository.AddAsync(entity);

            return new GlobalResponse
            {
                Status = 1,
                Message = "Tipo de afectación tributaria creado exitosamente.",
            };
        }
    }
}
