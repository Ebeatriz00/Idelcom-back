using Application.DTOs.ParentModules;
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

namespace Application.UseCases.ParentModules
{
    public class CreateParentModules
    {
        private readonly IParentModulesRepository _repository;
        private readonly IValidator<ParentModulesCreateDto> _validator;
        private readonly IMapper _mapper;

        public CreateParentModules(IParentModulesRepository repository, IValidator<ParentModulesCreateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }
        public async Task<GlobalResponse> ExecuteAsync(ParentModulesCreateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                    .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                    .ToList();
                throw new AppValidationException(errores);
            }

            var yaExiste = await _repository.ExistsAsync(dto.Title, dto.BusinessId);
            if (yaExiste)
                throw new DuplicateEntryException("El módulo padre ya existe para este negocio.");
            
            var entity = _mapper.Map<Core.Entities.ParentModules>(dto);
            await _repository.AddAsync(entity);
            return new GlobalResponse
            {
                Status = 1,
                Message = "Módulo padre creado exitosamente.",
            };
        }
    }
}
