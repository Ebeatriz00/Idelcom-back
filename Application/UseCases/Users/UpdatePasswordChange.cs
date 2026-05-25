using Application.DTOs.Users;
using Application.Exceptions;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Interfaces.Services;
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
    public class UpdatePasswordChange
    {
        private readonly IUsersRepository _repository;
        private readonly IValidator<UsersPasswordChangeDto> _validator;
        private readonly IMapper _mapper;
        private readonly IPasswordService _passwordService;

        public UpdatePasswordChange(IUsersRepository repository, IValidator<UsersPasswordChangeDto> validator, IMapper mapper, IPasswordService passwordService)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
            _passwordService = passwordService;
        }
        public async Task<GlobalResponse> ExecuteAsync(UsersPasswordChangeDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                            .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                            .ToList();
                throw new AppValidationException(errores);
            }
            
            var hashedPassword = _passwordService.HashPassword(dto.UsersPassword, out string salt);

            var entity = _mapper.Map<Core.Entities.Users>(dto);
            // Mapear y setear manualmente los campos críticos
            entity.UsersPassword = hashedPassword;
            entity.UsersSalt = salt;

            await _repository.PasswordChangeAsync(entity);
            return new GlobalResponse
            {
                Status = 1,
                Message = "Contraseña actualizada correctamente."
            };
        }
    }
}
