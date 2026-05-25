using Application.DTOs.Permissions;
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

namespace Application.UseCases.Permissions
{
    public class UpdatePermissions
    {

            private readonly IPermissionsRepository _repository;
            private readonly IValidator<PermissionsUpdateDto> _validator;
            private readonly IMapper _mapper;

            public UpdatePermissions(IPermissionsRepository repository, IValidator<PermissionsUpdateDto> validator, IMapper mapper)
            {
                _repository = repository;
                _validator = validator;
                _mapper = mapper;
            }

            public async Task<GlobalResponse> ExecuteAsync(PermissionsUpdateDto dto)
            {
                var validation = await _validator.ValidateAsync(dto);
                if (!validation.IsValid)
                {
                    var errores = validation.Errors
                        .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                        .ToList();
                    throw new AppValidationException(errores);
                }

                if (await _repository.ExistsAsync(dto.PermissionsName, dto.BusinessId, dto.PermissionsId))
                {
                    throw new DuplicateEntryException("El permiso ya existe para este negocio.");
                }

                var entity = _mapper.Map<Core.Entities.Permissions>(dto);
                var updated = await _repository.UpdateAsync(entity);
                return new GlobalResponse
                {
                    Status = updated ? 1 : 0,
                    Message = updated
                        ? "Permiso actualizado correctamente."
                        : "Error al actualizar el permiso."
                };
            }
    }
}
