using Application.DTOs.Business;
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

namespace Application.UseCases.Business
{
    public class CreateBusiness
    {
        private readonly IBusinessRepository _businessRepository;
        private readonly IValidator<BusinessCreateDto> _validator;
        private readonly IMapper _mapper;

        public CreateBusiness(IBusinessRepository businessRepository, IValidator<BusinessCreateDto> validator, IMapper mapper)
        {
            _businessRepository = businessRepository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(BusinessCreateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);

            if (!validation.IsValid)
            {
                var errores = validation.Errors
                           .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                           .ToList();
                throw new AppValidationException(errores);
            }

            var yaExiste = await _businessRepository.ExistsAsync(dto.BusinessRuc, dto.CodeLicense, dto.UsersBy);
            if (yaExiste)
                throw new Exceptions.DuplicateEntryException("El negocio ya existe.");

            var entity = _mapper.Map<Core.Entities.Business>(dto);
            await _businessRepository.AddAsync(entity);

            return new GlobalResponse
            {
                Status = 1,
                Message = "Negocio creado exitosamente.",
            };
        }
    }
}
