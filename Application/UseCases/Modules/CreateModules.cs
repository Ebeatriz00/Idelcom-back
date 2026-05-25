using Application.DTOs.Modules;
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

namespace Application.UseCases.Modules
{
    public class CreateModules
    {
        private readonly IModulesRepository _repository;
        private readonly IValidator<ModulesCreateDto> _validator;
        private readonly IMapper _mapper;

        public CreateModules(IModulesRepository repository, IValidator<ModulesCreateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(ModulesCreateDto dto)
        {
            
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                    .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                    .ToList();
                throw new AppValidationException(errores);
            }
            
            var yaExiste = await _repository.ExistsAsync(dto.Label, dto.BusinessId);
            if (yaExiste)
                throw new DuplicateEntryException("El módulo ya existe para este negocio.");

            
            var entity = _mapper.Map<Core.Entities.Modules>(dto);
            await _repository.AddAsync(entity);
            return new GlobalResponse
            {
                Status = 1,
                Message = "Módulo creado exitosamente.",
            };
        }
    }
}
