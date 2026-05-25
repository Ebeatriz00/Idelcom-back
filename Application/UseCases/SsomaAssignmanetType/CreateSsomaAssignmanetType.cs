using Application.DTOs.Ssoma;
using Application.Exceptions;
using AutoMapper;
using Core.Interfaces;
using FluentValidation;
using Org.BouncyCastle.Tsp;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppValidationException = Application.Exceptions.ValidationException;


namespace Application.UseCases.SsomaAssignmanetType
{
    public class CreateSsomaAssignmanetType
    {
        private readonly ISsomaAssignmanetTypeRepository _repository;
        private readonly IValidator<SsomaAssignmanetTypeCreateDto> _validator;
        private readonly IMapper _mapper;

        public CreateSsomaAssignmanetType(ISsomaAssignmanetTypeRepository ssomaAssignmanetTypeRepository, IValidator<SsomaAssignmanetTypeCreateDto> validator, IMapper mapper)
        {
            _repository = ssomaAssignmanetTypeRepository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(SsomaAssignmanetTypeCreateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                           .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                           .ToList();
                throw new AppValidationException(errores);
            }
            var yaExiste = await _repository.ExistsAsync(dto.SsomaAssignamentName, dto.BusinessId);
            if (yaExiste)
                throw new DuplicateEntryException("El tipo de asignación ya existe para este negocio.");

            var entity = _mapper.Map<Core.Entities.SsomaAssignmanetType>(dto);
            await _repository.AddAsync(entity);

            return new GlobalResponse
            {
                Status = 1,
                Message = "Tipo de asignación creado exitosamente.",
            };
        }   
    }
}
