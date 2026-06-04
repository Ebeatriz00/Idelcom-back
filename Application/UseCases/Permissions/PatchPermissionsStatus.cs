using Application.DTOs.Permissions;
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

namespace Application.UseCases.Permissions
{
    public class PatchPermissionsStatus
    {
            private readonly IPermissionsRepository _repository;
            private readonly IProfilesPermissionsRepository _profilesPermissionsRepository;
            private readonly IValidator<PermissionsStatusToggleDto> _validator;
            private readonly IAuthPermissionService _authPermissionService;

            public PatchPermissionsStatus(
                IPermissionsRepository repository,
                IProfilesPermissionsRepository profilesPermissionsRepository,
                IValidator<PermissionsStatusToggleDto> validator,
                IAuthPermissionService authPermissionService)
            {
                _repository = repository;
                _profilesPermissionsRepository = profilesPermissionsRepository;
                _validator = validator;
                _authPermissionService = authPermissionService;
            }

            public async Task<GlobalResponse> ExecuteAsync(PermissionsStatusToggleDto dto)
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
                    .GetAffectedProfileIdsByPermissionAsync(dto.PermissionsId, dto.BusinessId);
                var updated = await _repository.PatchStatusAsync(dto.PermissionsId, dto.Status, dto.UsersBy, dto.BusinessId);
                if (updated)
                {
                    foreach (var profilesId in affectedProfiles)
                        _authPermissionService.Invalidate(profilesId, dto.BusinessId);
                }

                return new GlobalResponse
                {
                    Status = updated ? 1 : 0,
                    Message = updated
                        ? "Estado del permiso actualizado correctamente."
                        : "No se pudo actualizar el estado del permiso."
                };
            }
        }

    }
