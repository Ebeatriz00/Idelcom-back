using Application.DTOs.MovOper;
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

namespace Application.UseCases.MovOper
{
    public class CreateMovOper
    {
        private readonly IMovOperRepository _repository;
        private readonly IValidator<MovOperCreateDto> _validator;
        private readonly IMapper _mapper;

        public CreateMovOper(IMovOperRepository repository, IValidator<MovOperCreateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(MovOperCreateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors.Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage)).ToList();
                throw new AppValidationException(errores);
            }

            if (await _repository.ExistsAsync(dto.Description, dto.BusinessId))
                throw new DuplicateEntryException("El tipo de operación ya existe para este negocio.");

            var entity = _mapper.Map<Core.Entities.MovOper>(dto);
            await _repository.AddAsync(entity);

            return new GlobalResponse { Status = 1, Message = "Tipo de operación creado exitosamente." };
        }
    }
}
