using Application.DTOs.CommercialParameters;
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


namespace Application.UseCases.CommercialParameters
{
    public class CreateCommercialParameters
    {
        private readonly ICommercialParametersRepository _repository;
        private readonly IValidator<CreateCommercialParametersDto> _validator;
        private readonly IMapper _mapper;

        public CreateCommercialParameters(ICommercialParametersRepository repository, IValidator<CreateCommercialParametersDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(CreateCommercialParametersDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                            .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                            .ToList();
                throw new AppValidationException(errores);
            }

            var yaExiste = await _repository.ExistsAsync(dto.ParametersName, dto.BusinessId);
            if (yaExiste)
                throw new DuplicateEntryException("El área ya existe para este negocio.");


            var entity = _mapper.Map<Core.Entities.CommercialParameters>(dto);
            await _repository.AddAsync(entity);
            return new GlobalResponse
            {
                Status = 1,
                Message = "Parámetros comerciales creados exitosamente.",
            };
        }
    }
}
