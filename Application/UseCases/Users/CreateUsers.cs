using Application.DTOs.Users;
using Application.Exceptions;
using AutoMapper;
using Core.Interfaces;
using Core.Interfaces.Services;
using FluentValidation;
using SharedKernel;
using System.Linq;
using System.Threading.Tasks;
using AppValidationException = Application.Exceptions.ValidationException;

namespace Application.UseCases.Users
{
    public class CreateUsers
    {
        private readonly IUsersRepository _repository;
        private readonly IValidator<UsersCreateDto> _validator;
        private readonly IMapper _mapper;
        private readonly IPasswordService _passwordService;

        public CreateUsers(
            IUsersRepository repository,
            IValidator<UsersCreateDto> validator,
            IMapper mapper,
            IPasswordService passwordService)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
            _passwordService = passwordService;
        }

        public async Task<GlobalResponse> ExecuteAsync(UsersCreateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                            .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                            .ToList();
                throw new AppValidationException(errores);
            }

            var yaExiste = await _repository.ExistsAsync(dto.UsersDocument, dto.BusinessId);
            if (yaExiste)
                throw new DuplicateEntryException("El usuario ya existe para este negocio.");

            var codExiste = await _repository.GetLastUserCodeAsync(dto.UsersCode, dto.BusinessId);
            if (codExiste)
                throw new DuplicateEntryException("El codigo de usuario ya existe para este negocio.");

            // Hash y salt con el servicio inyectado
            var hashedPassword = _passwordService.HashPassword(dto.UsersPassword, out string salt);

            // Mapear y setear manualmente los campos críticos
            var entity = _mapper.Map<Core.Entities.Users>(dto);
            entity.UsersPassword = hashedPassword;
            entity.UsersSalt = salt;

            await _repository.AddAsync(entity);

            return new GlobalResponse
            {
                Status = 1,
                Message = "Usuario creado exitosamente.",
            };
        }
    }
}
