using Application.DTOs.MovSunat;
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

namespace Application.UseCases.MovSunat
{
    public class CreateMovSunat
    {
        private readonly IMovSunatRepository _repository;
        private readonly IValidator<MovSunatCreateDto> _validator;
        private readonly IMapper _mapper;

        public CreateMovSunat(IMovSunatRepository repository, IValidator<MovSunatCreateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(MovSunatCreateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors.Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage)).ToList();
                throw new AppValidationException(errores);
            }

            if (await _repository.ExistsAsync(dto.Description, dto.BusinessId))
                throw new DuplicateEntryException("El tipo SUNAT ya existe para este negocio.");

            var entity = _mapper.Map<Core.Entities.MovSunat>(dto);
            await _repository.AddAsync(entity);

            return new GlobalResponse { Status = 1, Message = "Tipo SUNAT creado exitosamente." };
        }
    }
}
