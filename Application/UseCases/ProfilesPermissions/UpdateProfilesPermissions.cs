using Application.DTOs.ProfilesPermissions;
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

namespace Application.UseCases.ProfilesPermissions
{
    public class UpdateProfilesPermissions
    {
        private readonly IProfilesPermissionsRepository _repository;
        private readonly IValidator<ProfilesPermissionsUpdateDto> _validator;
        private readonly IMapper _mapper;
        private readonly IAuthPermissionService _authPermissionService;

        public UpdateProfilesPermissions(
            IProfilesPermissionsRepository repository,
            IValidator<ProfilesPermissionsUpdateDto> validator,
            IMapper mapper,
            IAuthPermissionService authPermissionService)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
            _authPermissionService = authPermissionService;
        }

        public async Task<GlobalResponse> ExecuteAsync(ProfilesPermissionsUpdateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                    .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                    .ToList();
                throw new Application.Exceptions.ValidationException(errores);
            }

            if (await _repository.ExistsAsync(dto.ProfilesId, dto.ModulesPermissionsId, dto.BusinessId, dto.ProfilesPermissionsId))
            {
                throw new DuplicateEntryException("El permiso de módulo ya existe para este negocio.");
            }
            var entity = _mapper.Map<Core.Entities.ProfilesPermissions>(dto);
            var updated = await _repository.UpdateAsync(entity);
            if (updated)
                _authPermissionService.Invalidate(dto.ProfilesId, dto.BusinessId);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                    ? "Permiso de perfil actualizado correctamente."
                    : "Error al actualizar el permiso de perfil."
            };
        }
    }
}
