using Application.DTOs.MovVis;
using Core.Interfaces;
using FluentValidation;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppValidationException = Application.Exceptions.ValidationException;

namespace Application.UseCases.MovVis
{
    public class PatchMovVisStatus
    {
        private readonly IMovVisRepository _repository;
        private readonly IValidator<MovVisStatusToggleDto> _validator;

        public PatchMovVisStatus(IMovVisRepository repository, IValidator<MovVisStatusToggleDto> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<GlobalResponse> ExecuteAsync(MovVisStatusToggleDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors.Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage)).ToList();
                throw new AppValidationException(errores);
            }

            var updated = await _repository.PatchStatusAsync(dto.MovVisId, dto.Status, dto.UsersBy, dto.BusinessId);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated ? "Estado de la visibilidad de movimiento actualizado correctamente." : "No se pudo actualizar el estado de la visibilidad de movimiento."
            };
        }
    }
}
