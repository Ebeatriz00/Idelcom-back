using Application.DTOs.ParentModules;
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
    public class UpdateParentModules
    {
        private readonly IParentModulesRepository _repository;
        private readonly IValidator<ParentModulesUpdateDto> _validator;
        private readonly IMapper _mapper;
        public UpdateParentModules(IParentModulesRepository repository, IValidator<ParentModulesUpdateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }
        public async Task<GlobalResponse> ExecuteAsync(ParentModulesUpdateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                .ToList();
                throw new AppValidationException(errores);
            }
            if (await _repository.ExistsAsync(dto.Title, dto.BusinessId, dto.ParentModulesId))
            {
                throw new Exceptions.DuplicateEntryException("El módulo padre ya existe para este negocio.");
            }
            var entity = _mapper.Map<Core.Entities.ParentModules>(dto);
            var updated = await _repository.UpdateAsync(entity);
            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                ? "Módulo padre actualizado correctamente."
                : "Error al actualizar el módulo padre."
            };
        }
    }
}
