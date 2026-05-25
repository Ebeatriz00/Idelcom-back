using Application.DTOs.UsersPreferences;
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


namespace Application.UseCases.UsersPreferences
{
    public class UpdateSettingUsersPrefe
    {
        private readonly IUsersPreferencesRepository _repository;
        private readonly IValidator<UsersPrefeSettingUpdateDto> _validator;
        private readonly IMapper _mapper;

        public UpdateSettingUsersPrefe(IUsersPreferencesRepository repository, IValidator<UsersPrefeSettingUpdateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }
        public async Task<GlobalResponse> ExecuteAsync(UsersPrefeSettingUpdateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                            .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                            .ToList();
                throw new AppValidationException(errores);
            }

            var entity = _mapper.Map<Core.Entities.UsersPreferences>(dto);
            await _repository.UpdateSettingAsync(entity);

            return new GlobalResponse
            {
                Status = 1,
                Message = "Preferecias actualizado exitosamente.",
            };
        }
    }
}
