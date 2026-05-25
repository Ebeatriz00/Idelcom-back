using Application.DTOs.Suppliers;
using Core.Interfaces.Logistic;
using FluentValidation;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppValidationException = Application.Exceptions.ValidationException;


namespace Application.UseCases.Suppliers
{
    public class PatchSuppliersStatus
    {
        private readonly ISuppliersRepository _repository;
        private readonly IValidator<SuppliersStatusToggleDto> _validator;

        public PatchSuppliersStatus(
            ISuppliersRepository repository,
            IValidator<SuppliersStatusToggleDto> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<GlobalResponse> ExecuteAsync(SuppliersStatusToggleDto dto, long userId, long businessId)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                                        .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                                        .ToList();
                throw new AppValidationException(errores);
            }

            var updated = await _repository.PatchStatusAsync(
                dto.SuppliersId,
                dto.Status,
                userId,
                businessId
            );

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                    ? "Estado del proveedor actualizado correctamente."
                    : "No se pudo actualizar el estado del proveedor."
            };
        }
    }
}
