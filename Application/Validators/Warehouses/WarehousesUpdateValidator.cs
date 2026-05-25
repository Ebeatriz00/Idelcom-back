using Application.DTOs.Warehouses;
using FluentValidation;
using SharedKernel.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.Warehouses
{
    public class WarehousesUpdateValidator : AbstractValidator<WarehousesUpdateDto>
    {
        public WarehousesUpdateValidator()
        {
            RuleFor(x => x.WarehousesId)
                .GreaterThan(0).WithMessage("ID de almacén inválido.")
                .WithErrorCode(ErrorCodes.Conflict);

            
            RuleFor(x => x.DepartmentId)
                .GreaterThan(0).WithMessage("El departamento es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.ProvinceId)
                .GreaterThan(0).WithMessage("La provincia es obligatoria.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.DistrictId)
                .GreaterThan(0).WithMessage("El distrito es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            
            RuleFor(x => x.Description)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La descripción es obligatoria.")
                .WithErrorCode(ErrorCodes.ValidationEmpty)

                .MaximumLength(50).WithMessage("La descripción no debe exceder los 50 caracteres.")
                .WithErrorCode(ErrorCodes.ValidationLength)

                .Matches(@"^[a-zA-Z0-9\s\-,.áéíóúÁÉÍÓÚñÑ()]+$")
                .WithMessage("La descripción contiene caracteres inválidos.")
                .WithErrorCode(ErrorCodes.ValidationCharacterInvalid);

            RuleFor(x => x.Address)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La dirección es obligatoria.")
                .WithErrorCode(ErrorCodes.ValidationEmpty)

                .MaximumLength(50).WithMessage("La dirección no debe exceder los 50 caracteres.")
                .WithErrorCode(ErrorCodes.ValidationLength);
        }
    }
}
