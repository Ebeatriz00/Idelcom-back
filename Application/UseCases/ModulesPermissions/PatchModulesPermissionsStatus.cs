using Application.DTOs.ModulePermission;
using Application.Services.InterfacesServices;
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
    public class PatchModulesPermissionsStatus
    {
        private readonly IModulesPermissionsRepository _repository;
        private readonly IProfilesPermissionsRepository _profilesPermissionsRepository;
        private readonly IValidator<ModulesPermissionsStatusToggleDto> _validator;
        private readonly IAuthPermissionService _authPermissionService;

        public PatchModulesPermissionsStatus(
            IModulesPermissionsRepository repository,
            IProfilesPermissionsRepository profilesPermissionsRepository,
            IValidator<ModulesPermissionsStatusToggleDto> validator,
            IAuthPermissionService authPermissionService)
        {
            _repository = repository;
            _profilesPermissionsRepository = profilesPermissionsRepository;
            _validator = validator;
            _authPermissionService = authPermissionService;
        }

        public async Task<GlobalResponse> ExecuteAsync(ModulesPermissionsStatusToggleDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                    .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                    .ToList();
                throw new AppValidationException(errores);
            }

            var affectedProfiles = await _profilesPermissionsRepository
                .GetAffectedProfileIdsByModulesPermissionAsync(dto.ModulesPermissionsId, dto.BusinessId);

            var updated = await _repository.PatchStatusAsync(
                dto.ModulesPermissionsId,
                dto.Status,
                dto.UsersBy,
                dto.BusinessId
            );
            if (updated)
            {
                foreach (var profilesId in affectedProfiles)
                    _authPermissionService.Invalidate(profilesId, dto.BusinessId);
            }

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                    ? "Estado del permiso de módulo actualizado correctamente."
                    : "No se pudo actualizar el estado del permiso de módulo."
            };
        }
    }
}
