using Application.DTOs.ProcessType;
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

namespace Application.UseCases.ProcessType
{
    public class CreateProcessType
    {
        private readonly IProcessTypeReporsitory _repository;
        private readonly IValidator<ProcessTypeCreateDto> _validator;
        private readonly IMapper _mapper;

        public CreateProcessType(IProcessTypeReporsitory repository, IValidator<ProcessTypeCreateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(ProcessTypeCreateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                           .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                           .ToList();
                throw new AppValidationException(errores);
            }
            var yaExiste = await _repository.ExistsAsync(dto.DescType, dto.BusinessId);
            if (yaExiste)
                throw new DuplicateEntryException("El tipo de proceso ya existe para este negocio.");

            var entity = _mapper.Map<Core.Entities.ProcessType>(dto);
            await _repository.AddAsync(entity);

            return new GlobalResponse
            {
                Status = 1,
                Message = "Tipo de proceso creado exitosamente.",
            };
        }
    }
}
