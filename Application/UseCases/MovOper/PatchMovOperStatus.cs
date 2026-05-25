using Application.DTOs.MovOper;
using Core.Interfaces;
using FluentValidation;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppValidationException = Application.Exceptions.ValidationException;

namespace Application.UseCases.MovOper
{
    public class PatchMovOperStatus
    {
        private readonly IMovOperRepository _repository;
        private readonly IValidator<MovOperStatusToggleDto> _validator;

        public PatchMovOperStatus(IMovOperRepository repository, IValidator<MovOperStatusToggleDto> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<GlobalResponse> ExecuteAsync(MovOperStatusToggleDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors.Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage)).ToList();
                throw new AppValidationException(errores);
            }

            var updated = await _repository.PatchStatusAsync(dto.MovOperId, dto.Status, dto.UsersBy, dto.BusinessId);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated ? "Estado del tipo de operación actualizado correctamente." : "No se pudo actualizar el estado del tipo de operación."
            };
        }
    }
}
