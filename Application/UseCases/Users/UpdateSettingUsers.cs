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
    public class UpdateUsers
    {
        private readonly IUsersRepository _repository;
        private readonly IValidator<UsersUpdateDto> _validator;
        private readonly IMapper _mapper;
        public UpdateUsers(IUsersRepository repository, IValidator<UsersUpdateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }
        public async Task<GlobalResponse> ExecuteAsync(UsersUpdateDto dto)
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

            var codExiste = await _repository.GetLastUserCodeAsync(dto.UsersCode, dto.BusinessId, dto.UsersId);
            if (codExiste)
                throw new DuplicateEntryException("El codigo de usuario ya existe para este negocio.");

            var entity = _mapper.Map<Core.Entities.Users>(dto);
            await _repository.UpdateAsync(entity);
            return new GlobalResponse
            {
                Status = 1,
                Message = "Usuario actualizado exitosamente.",
            };
        }
    }
}
