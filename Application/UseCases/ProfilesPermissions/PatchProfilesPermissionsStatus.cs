using Application.DTOs.ProfilesPermissions;
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


namespace Application.UseCases.ProfilesPermissions
{
    public class PatchProfilesPermissionsStatus
    {
        private readonly IProfilesPermissionsRepository _repository;
        private readonly IValidator<ProfilesPermissionsStatusToggleDto> _validator;
        private readonly IAuthPermissionService _authPermissionService;

        public PatchProfilesPermissionsStatus(
            IProfilesPermissionsRepository repository,
            IValidator<ProfilesPermissionsStatusToggleDto> validator,
            IAuthPermissionService authPermissionService)
        {
            _repository = repository;
            _validator = validator;
            _authPermissionService = authPermissionService;
        }

        public async Task<GlobalResponse> ExecuteAsync(ProfilesPermissionsStatusToggleDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                    .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                    .ToList();
                throw new AppValidationException(errores);
            }

            var current = await _repository.GetByIdAsync(dto.ProfilesPermissionsId);
            var updated = await _repository.PatchStatusAsync(dto.ProfilesPermissionsId, dto.Status, dto.UsersBy, dto.BusinessId);
            if (updated && current is not null)
                _authPermissionService.Invalidate(current.ProfilesId, dto.BusinessId);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                    ? "Estado del permiso de perfil actualizado correctamente."
                    : "No se pudo actualizar el estado del permiso de perfil."
            };
        }
    }
}
