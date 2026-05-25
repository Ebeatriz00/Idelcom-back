using Application.DTOs.Profiles;
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


namespace Application.UseCases.Profiles
{
    public class UpdateProfiles
    {
        private readonly IProfilesRepository _repository;
        private readonly IValidator<ProfilesUpdateDto> _validator;
        private readonly IMapper _mapper;
        public UpdateProfiles(IProfilesRepository repository, IValidator<ProfilesUpdateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }
        public async Task<GlobalResponse> ExecuteAsync(ProfilesUpdateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                .ToList();
                throw new AppValidationException(errores);
            }
            if (await _repository.ExistsAsync(dto.Description, dto.BusinessId, dto.ProfilesId))
            {
                throw new Exceptions.DuplicateEntryException("El perfil ya existe para este negocio.");
            }
            var entity = _mapper.Map<Core.Entities.Profiles>(dto);
            var updated = await _repository.UpdateAsync(entity);
            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                ? "Perfil actualizado correctamente."
                : "Error al actualizar el perfil."
            };
        }
    }
}
