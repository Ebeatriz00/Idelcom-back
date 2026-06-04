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
using AppValidationException = Application.Exceptions.ValidationException;

namespace Application.UseCases.ProfilesPermissions
{
    public class CreateProfilesPermissions
    {
        private readonly IProfilesPermissionsRepository _repository;
        private readonly IValidator<ProfilesPermissionsCreateDto> _validator;
        private readonly IMapper _mapper;
        private readonly IAuthPermissionService _authPermissionService;

        public CreateProfilesPermissions(
            IProfilesPermissionsRepository repository,
            IValidator<ProfilesPermissionsCreateDto> validator,
            IMapper mapper,
            IAuthPermissionService authPermissionService)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
            _authPermissionService = authPermissionService;
        }

        public async Task<GlobalResponse> ExecuteAsync(ProfilesPermissionsCreateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                    .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                    .ToList();

                throw new AppValidationException(errores);
            }

            var entities = new List<Core.Entities.ProfilesPermissions>();

            foreach (var modulePermId in dto.ModulesPermissionsId)
            {
                var exists = await _repository.ExistsAsync(dto.ProfilesId, modulePermId, dto.BusinessId);
                if (exists)
                    throw new DuplicateEntryException($"El permiso {modulePermId} ya existe para este perfil.");

                entities.Add(new Core.Entities.ProfilesPermissions
                {
                    BusinessId = dto.BusinessId,
                    ProfilesId = dto.ProfilesId,
                    ModulesPermissionsId =  modulePermId ,
                    UsersBy = dto.UsersBy
                });
            }

            await _repository.AddAsync(entities);
            _authPermissionService.Invalidate(dto.ProfilesId, dto.BusinessId);

            return new GlobalResponse
            {
                Status = 1,
                Message = "Permisos registrados exitosamente."
            };
        }
    }
    }
