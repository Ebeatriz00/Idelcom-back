using Application.DTOs.ProductLines;
using FluentValidation;
using SharedKernel.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.ProductLines
{
    public class ProductLinesStatusToggleValidator : AbstractValidator<ProductLinesStatusToggleDto>
    {
        public ProductLinesStatusToggleValidator()
        {
            RuleFor(x => x.ProductLinesId)
                .GreaterThan(0).WithMessage("ID de línea de producto inválido.")
                .WithErrorCode(ErrorCodes.Conflict);

            RuleFor(x => x.BusinessId)
                .GreaterThan(0).WithMessage("El negocio es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.Status)
                .Must(status => status == "1" || status == "0")
                .WithMessage("Estado inválido. Solo se permite '1' (Activo) o '0' (Inactivo).")
                .WithErrorCode(ErrorCodes.Conflict)

                .Matches(@"^[a-zA-Z0-9\s\-,.áéíóúÁÉÍÓÚñÑ()]+$")
                .WithMessage("El estado tiene caracteres especiales.")
                .WithErrorCode(ErrorCodes.ValidationCharacterInvalid);


           
        }
    }
}
