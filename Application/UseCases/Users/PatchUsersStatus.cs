using Application.DTOs.Currency;
using Application.DTOs.Users;
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
    public class PatchUsersStatus
    {
        private readonly IUsersRepository _repository;
        private readonly IValidator<UsersStatusToggleDto> _validator;

        public PatchUsersStatus(IUsersRepository repository, IValidator<UsersStatusToggleDto> validator)
        {
            _repository = repository;
            _validator = validator;
        }
        public async Task<GlobalResponse> ExecuteAsync(UsersStatusToggleDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                .ToList();

                throw new AppValidationException(errores);

            }

            var updated = await _repository.PatchStatusAsync(dto.UsersId, dto.Status, dto.UsersBy, dto.BusinessId);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                ? "Estado del usuario actualizado correctamente."
                : "No se pudo actualizar el estado del usuario."
            };
        }
    }
}
