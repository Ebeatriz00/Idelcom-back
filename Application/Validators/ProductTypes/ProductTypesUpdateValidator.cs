using Application.DTOs.ProductTypes;
using FluentValidation;
using SharedKernel.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.ProductTypes
{
    public class ProductTypesUpdateValidator : AbstractValidator<ProductTypesUpdateDto>
    {
        public ProductTypesUpdateValidator()
        {
            RuleFor(x => x.ProductTypesId)
                .GreaterThan(0).WithMessage("ID de tipo de producto inválido.")
                .WithErrorCode(ErrorCodes.Conflict);

            RuleFor(x => x.Description)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("La descripción es obligatoria.")
                .WithErrorCode(ErrorCodes.ValidationEmpty)

                .MaximumLength(50)
                .WithMessage("La descripción no debe exceder los 50 caracteres.")
                .WithErrorCode(ErrorCodes.ValidationLength)

                .Matches(@"^[a-zA-Z0-9\s\-,.áéíóúÁÉÍÓÚñÑ()]+$")
                .WithMessage("La descripción contiene caracteres inválidos.")
                .WithErrorCode(ErrorCodes.ValidationCharacterInvalid);
        }
    }
}
