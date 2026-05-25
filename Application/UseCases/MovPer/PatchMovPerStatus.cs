using Application.DTOs.MovPer;
using Core.Interfaces;
using FluentValidation;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppValidationException = Application.Exceptions.ValidationException;

namespace Application.UseCases.MovPer
{
    public class PatchMovPerStatus
    {
        private readonly IMovPerRepository _repository;
        private readonly IValidator<MovPerStatusToggleDto> _validator;

        public PatchMovPerStatus(IMovPerRepository repository, IValidator<MovPerStatusToggleDto> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<GlobalResponse> ExecuteAsync(MovPerStatusToggleDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors.Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage)).ToList();
                throw new AppValidationException(errores);
            }

            var updated = await _repository.PatchStatusAsync(dto.MovPerId, dto.Status, dto.UsersBy, dto.BusinessId);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated ? "Estado del periodo de movimiento actualizado correctamente." : "No se pudo actualizar el estado del periodo de movimiento."
            };
        }
    }
}
