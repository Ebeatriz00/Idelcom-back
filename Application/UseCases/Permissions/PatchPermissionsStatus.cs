using Application.DTOs.Permissions;
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
            private readonly IValidator<PermissionsStatusToggleDto> _validator;

            public PatchPermissionsStatus(IPermissionsRepository repository, IValidator<PermissionsStatusToggleDto> validator)
            {
                _repository = repository;
                _validator = validator;
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

                var updated = await _repository.PatchStatusAsync(dto.PermissionsId, dto.Status, dto.UsersBy, dto.BusinessId);

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
