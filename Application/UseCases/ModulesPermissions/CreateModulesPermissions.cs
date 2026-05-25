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
    public class CreateModulesPermissions
    {
        private readonly IModulesPermissionsRepository _repository;
        private readonly IValidator<ModulesPermissionsCreateDto> _validator;
        private readonly IMapper _mapper;

        public CreateModulesPermissions(
            IModulesPermissionsRepository repository,
            IValidator<ModulesPermissionsCreateDto> validator,
            IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(ModulesPermissionsCreateDto dto)
        {
            // 1. Validación del DTO
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                    .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                    .ToList();

                throw new AppValidationException(errores);
            }

            // 2. Verificar duplicados (ejemplo: por ModuleId y PermissionId)
            var yaExiste = await _repository.ExistsAsync(dto.ModulesId, dto.PermissionsId, dto.BusinessId);
            if (yaExiste)
                throw new DuplicateEntryException("El permiso ya existe para este módulo y negocio.");

            // 3. Mapear y guardar
            var entity = _mapper.Map<Core.Entities.ModulesPermissions>(dto);
            await _repository.AddAsync(entity);

            // 4. Respuesta global
            return new GlobalResponse
            {
                Status = 1,
                Message = "Permiso de módulo creado exitosamente.",
            };
        }
    }
}
