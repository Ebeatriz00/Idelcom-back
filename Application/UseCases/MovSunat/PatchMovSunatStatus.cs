using Application.DTOs.MovSunat;
using Core.Interfaces;
using FluentValidation;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppValidationException = Application.Exceptions.ValidationException;

namespace Application.UseCases.MovSunat
{
    public class PatchMovSunatStatus
    {
        private readonly IMovSunatRepository _repository;
        private readonly IValidator<MovSunatStatusToggleDto> _validator;

        public PatchMovSunatStatus(IMovSunatRepository repository, IValidator<MovSunatStatusToggleDto> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<GlobalResponse> ExecuteAsync(MovSunatStatusToggleDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors.Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage)).ToList();
                throw new AppValidationException(errores);
            }

            var updated = await _repository.PatchStatusAsync(dto.MovSunatId, dto.Status, dto.UsersBy, dto.BusinessId);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated ? "Estado del tipo SUNAT actualizado correctamente." : "No se pudo actualizar el estado del tipo SUNAT."
            };
        }
    }
}
