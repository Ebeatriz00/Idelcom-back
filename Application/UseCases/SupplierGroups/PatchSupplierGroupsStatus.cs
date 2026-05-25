using Application.DTOs.SupplierGroups;
using Core.Interfaces.Logistic;
using FluentValidation;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppValidationException = Application.Exceptions.ValidationException;


namespace Application.UseCases.SupplierGroups
{
    public class PatchSupplierGroupsStatus
    {
        private readonly ISupplierGroupsRepository _repository;
        private readonly IValidator<SupplierGroupsStatusToggleDto> _validator;

        public PatchSupplierGroupsStatus(ISupplierGroupsRepository repository, IValidator<SupplierGroupsStatusToggleDto> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<GlobalResponse> ExecuteAsync(SupplierGroupsStatusToggleDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                                .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                                .ToList();

                throw new AppValidationException(errores);
            }

            var updated = await _repository.PatchStatusAsync(dto.SupplierGroupsId, dto.Status, dto.UsersBy, dto.BusinessId);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                ? "Estado del grupo de proveedores actualizado correctamente."
                : "No se pudo actualizar el estado del grupo de proveedores."
            };
        }
    }
}
