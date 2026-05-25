using Application.DTOs.BusinessLine;
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


namespace Application.UseCases.BusinessLine
{
    public class CreateBusinessLine
    {
        private readonly IBusinessLineRepository _repository;
        private readonly IValidator<BusinessLineCreateDto> _validator;
        private readonly IMapper _mapper;

        public CreateBusinessLine(IBusinessLineRepository repository, IValidator<BusinessLineCreateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(BusinessLineCreateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                           .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                           .ToList();
                throw new AppValidationException(errores);
            }
            var yaExiste = await _repository.ExistsAsync(dto.DescLine, dto.BusinessId);
            if (yaExiste)
                throw new DuplicateEntryException("La linea de negocio ya existe para este negocio.");

            var entity = _mapper.Map<Core.Entities.BusinessLine>(dto);
            await _repository.AddAsync(entity);

            return new GlobalResponse
            {
                Status = 1,
                Message = "Linea de negocio creado exitosamente.",
            };
        }
    }
}

