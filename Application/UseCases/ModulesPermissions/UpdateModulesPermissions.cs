using Application.DTOs.ModulePermission;
using Application.Exceptions;
using Application.Services.InterfacesServices;
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
        private readonly IProfilesPermissionsRepository _profilesPermissionsRepository;
        private readonly IValidator<ModulesPermissionsUpdateDto> _validator;
        private readonly IMapper _mapper;
        private readonly IAuthPermissionService _authPermissionService;

        public UpdateModulesPermissions(
            IModulesPermissionsRepository repository,
            IProfilesPermissionsRepository profilesPermissionsRepository,
            IValidator<ModulesPermissionsUpdateDto> validator,
            IMapper mapper,
            IAuthPermissionService authPermissionService)
        {
            _repository = repository;
            _profilesPermissionsRepository = profilesPermissionsRepository;
            _validator = validator;
            _mapper = mapper;
            _authPermissionService = authPermissionService;
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
            if (await _repository.ExistsAsync(dto.ModulesId, dto.PermissionsId, dto.BusinessId, dto.ModulesPermissionsId))
            {
                throw new DuplicateEntryException("El permiso de módulo ya existe para este negocio.");
            }

            var entity = _mapper.Map<Core.Entities.ModulesPermissions>(dto);
            var affectedProfiles = await _profilesPermissionsRepository
                .GetAffectedProfileIdsByModulesPermissionAsync(dto.ModulesPermissionsId, dto.BusinessId);
            var updated = await _repository.UpdateAsync(entity);
            if (updated)
            {
                foreach (var profilesId in affectedProfiles)
                    _authPermissionService.Invalidate(profilesId, dto.BusinessId);
            }

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
