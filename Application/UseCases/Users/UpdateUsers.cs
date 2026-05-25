using Application.DTOs.Users;
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

namespace Application.UseCases.Users
{
    public class UpdateSettingUsers
    {
        private readonly IUsersRepository _repository;
        private readonly IValidator<UsersSettingUpdateDto> _validator;
        private readonly IMapper _mapper;
        public UpdateSettingUsers(IUsersRepository repository, IValidator<UsersSettingUpdateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }
        public async Task<GlobalResponse> ExecuteAsync(UsersSettingUpdateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                            .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                            .ToList();
                throw new AppValidationException(errores);
            }

            if (await _repository.ExistsAsync(dto.UsersDocument,  dto.BusinessId, dto.UsersId))
            {
                throw new DuplicateEntryException("El usuario ya existe para este negocio.");

            }
            var entity = _mapper.Map<Core.Entities.Users>(dto);
            await _repository.UpdateSettingAsync(entity);
            return new GlobalResponse
            {
                Status = 1,
                Message = "Usuario actualizado exitosamente.",
            };
        }
    }
}
