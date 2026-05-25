using Application.DTOs.ModulePermission;
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


namespace Application.UseCases.ModulePermission
{
    public class UpdateModulesPermissions
    {
        private readonly IModulesPermissionsRepository _repository;
        private readonly IValidator<ModulesPermissionsUpdateDto> _validator;
        private readonly IMapper _mapper;

        public UpdateModulesPermissions(
            IModulesPermissionsRepository repository,
            IValidator<ModulesPermissionsUpdateDto> validator,
            IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(ModulesPermissionsUpdateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                    .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                    .ToList();
                throw new AppValidationException(errores);
            }

            // Verifica duplicados considerando ID y negocio
            if (await _repository.ExistsAsync(dto.ModulesId,dto.PermissionsId, dto.BusinessId, dto.ModulesPermissionsId))
            {
                throw new DuplicateEntryException("El permiso de módulo ya existe para este negocio.");
            }

            var entity = _mapper.Map<Core.Entities.ModulesPermissions>(dto);
            var updated = await _repository.UpdateAsync(entity);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                    ? "Permiso de módulo actualizado correctamente."
                    : "Error al actualizar el permiso de módulo."
            };
        }
    }
}
