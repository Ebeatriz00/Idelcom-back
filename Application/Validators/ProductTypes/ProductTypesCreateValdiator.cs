using Application.DTOs.ProductTypes;
using FluentValidation;
using SharedKernel.Constants;

namespace Application.Validators.ProductTypes
{
    public class ProductTypesCreateValidator
        : AbstractValidator<ProductTypesCreateDto>
    {
        public ProductTypesCreateValidator()
        {
           
            RuleFor(x => x.Description)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                    .WithMessage("La descripción es obligatoria.")
                    .WithErrorCode(ErrorCodes.ValidationEmpty)

                .MinimumLength(10)
                    .WithMessage("La descripción debe tener al menos 10 caracteres.")
                    .WithErrorCode(ErrorCodes.ValidationLength)

                .MaximumLength(50)
                    .WithMessage("La descripción no debe exceder 50 caracteres.")
                    .WithErrorCode(ErrorCodes.ValidationLength)

                .Matches(@"^[a-zA-Z0-9\s\-,.áéíóúÁÉÍÓÚñÑ()]+$")
                    .WithMessage("La descripción contiene caracteres inválidos.")
                    .WithErrorCode(ErrorCodes.ValidationCharacterInvalid);


            // Regla: consumible no puede ser retornable
            RuleFor(x => x)
                .Must(x => !(x.IsConsumable && x.IsReturnable))
                .WithMessage("Un producto consumible no puede ser retornable.")
                .WithErrorCode(ErrorCodes.ValidationConflict);


            // Regla: serializado no puede ser consumible
            RuleFor(x => x)
                .Must(x => !(x.RequiresSerial && x.IsConsumable))
                .WithMessage("Un producto serializado no puede ser consumible.")
                .WithErrorCode(ErrorCodes.ValidationConflict);


            // Opcional:
            // Si requiere serie, debería ser retornable
            RuleFor(x => x)
                .Must(x => !x.RequiresSerial || x.IsReturnable)
                .WithMessage("Un producto serializado debe ser retornable.")
                .WithErrorCode(ErrorCodes.ValidationConflict);
        }
    }
}