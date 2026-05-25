using Application.DTOs.Profiles;
using Core.Interfaces;
using FluentValidation;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppValidationException = Application.Exceptions.ValidationException;

namespace Application.UseCases.Profiles
{
    public class PatchProfilesStatus
    {
        private readonly IProfilesRepository _repository;
        private readonly IValidator<ProfilesStatusToggleDto> _validator;
        public PatchProfilesStatus(IProfilesRepository repository, IValidator<ProfilesStatusToggleDto> validator)
        {
            _repository = repository;
            _validator = validator;
        }
        public async Task<GlobalResponse> ExecuteAsync(ProfilesStatusToggleDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                .ToList();
                throw new AppValidationException(errores);
            }
            var updated = await _repository.PatchStatusAsync(dto.ProfilesId, dto.Status, dto.UsersBy, dto.BusinessId);
            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                ? "Estado del perfil actualizado correctamente."
                : "No se pudo actualizar el estado del perfil."
            };
        }
    }
}
