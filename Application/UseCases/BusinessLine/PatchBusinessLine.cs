using Application.DTOs.BusinessLine;
using Core.Interfaces;
using FluentValidation;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppValidationException = Application.Exceptions.ValidationException;

namespace Application.UseCases.BusinessLine
{
    public class PatchBusinessLine
    {
        private readonly IBusinessLineRepository _repository;
        private readonly IValidator<BusinessLineStatusToggleDto> _validator;

        public PatchBusinessLine(IBusinessLineRepository repository, IValidator<BusinessLineStatusToggleDto> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<GlobalResponse> ExecuteAsync(BusinessLineStatusToggleDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                .ToList();
                throw new AppValidationException(errores);
            }

            var updated = await _repository.PatchStatusAsync(dto.BusinessLineId, dto.Status, dto.UsersBy, dto.BusinessId);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                ? "Estado actualizado correctamente."
                : "No se pudo actualizar el estado."
            };
        }
    }
}
