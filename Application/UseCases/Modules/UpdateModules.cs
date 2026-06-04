using Application.DTOs.Modules;
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


namespace Application.UseCases.Modules
{
    public class UpdateModules
    {
        private readonly IModulesRepository _repository;
        private readonly IProfilesPermissionsRepository _profilesPermissionsRepository;
        private readonly IValidator<ModulesUpdateDto> _validator;
        private readonly IMapper _mapper;
        private readonly IAuthPermissionService _authPermissionService;

        public UpdateModules(
            IModulesRepository repository,
            IProfilesPermissionsRepository profilesPermissionsRepository,
            IValidator<ModulesUpdateDto> validator,
            IMapper mapper,
            IAuthPermissionService authPermissionService)
        {
            _repository = repository;
            _profilesPermissionsRepository = profilesPermissionsRepository;
            _validator = validator;
            _mapper = mapper;
            _authPermissionService = authPermissionService;
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
            var affectedProfiles = await _profilesPermissionsRepository
                .GetAffectedProfileIdsByModuleAsync(dto.ModulesId, dto.BusinessId);
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
                ? "Módulo actualizado correctamente."
                : "Error al actualizar el módulo."
            };
        }
    }
}
