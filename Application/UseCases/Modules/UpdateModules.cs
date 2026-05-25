using Application.DTOs.Modules;
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
    public class UpdateModules
    {
        private readonly IModulesRepository _repository;
        private readonly IValidator<ModulesUpdateDto> _validator;
        private readonly IMapper _mapper;
        public UpdateModules(IModulesRepository repository, IValidator<ModulesUpdateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }
        public async Task<GlobalResponse> ExecuteAsync(ModulesUpdateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                .ToList();
                throw new AppValidationException(errores);
            }
            if (await _repository.ExistsAsync(dto.Label, dto.BusinessId, dto.ModulesId))
            {
                throw new Exceptions.DuplicateEntryException("El módulo ya existe para este negocio.");
            }
            var entity = _mapper.Map<Core.Entities.Modules>(dto);
            var updated = await _repository.UpdateAsync(entity);
            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                ? "Módulo actualizado correctamente."
                : "Error al actualizar el módulo."
            };
        }
    }
}
