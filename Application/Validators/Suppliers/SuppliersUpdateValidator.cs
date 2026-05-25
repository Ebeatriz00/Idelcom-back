using Application.DTOs.Suppliers;
using FluentValidation;
using SharedKernel.Constants;

namespace Application.Validators.Suppliers
{
    public class SuppliersUpdateValidator : AbstractValidator<SuppliersUpdateDto>
    {
        public SuppliersUpdateValidator()
        {
            RuleFor(x => x.SuppliersId)
                .GreaterThan(0).WithMessage("ID de proveedor invalido.")
                .WithErrorCode(ErrorCodes.Conflict);

            RuleFor(x => x.SupplierTypeId)
                .GreaterThan(0).WithMessage("El tipo de proveedor es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.SupplierGroupsId)
                .GreaterThan(0).WithMessage("El grupo de proveedor es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.PaymentConditionId)
                .GreaterThan(0).WithMessage("La condicion de pago es obligatoria.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.PaymentMethodId)
                .GreaterThan(0).WithMessage("El metodo de pago es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.DocumentTypeId)
                .GreaterThan(0).WithMessage("El tipo de documento es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.DepartmentId)
                .NotNull().WithMessage("El departamento es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationEmpty)
                .GreaterThan(0).WithMessage("El departamento es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.ProvinceId)
                .NotNull().WithMessage("La provincia es obligatoria.")
                .WithErrorCode(ErrorCodes.ValidationEmpty)
                .GreaterThan(0).WithMessage("La provincia es obligatoria.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.DistrictId)
                .NotNull().WithMessage("El distrito es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationEmpty)
                .GreaterThan(0).WithMessage("El distrito es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.SupplierName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre de proveedor es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationEmpty)
                .MaximumLength(250).WithMessage("El nombre de proveedor no debe exceder los 250 caracteres.")
                .WithErrorCode(ErrorCodes.ValidationLength)
                .Matches(@"^[a-zA-Z0-9\s\.áéíóúÁÉÍÓÚñÑ'&()\-]+$")
                .WithMessage("El nombre de proveedor contiene caracteres invalidos.")
                .WithErrorCode(ErrorCodes.ValidationCharacterInvalid);


            RuleFor(x => x.ContactName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre de contacto es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationEmpty)
                .MaximumLength(150).WithMessage("El nombre de contacto no debe exceder los 150 caracteres.")
                .WithErrorCode(ErrorCodes.ValidationLength)
                .Matches(@"^[a-zA-Z\s\.áéíóúÁÉÍÓÚñÑ']+$")
                .WithMessage("El nombre de contacto contiene caracteres invalidos.")
                .WithErrorCode(ErrorCodes.ValidationCharacterInvalid);

            RuleFor(x => x.DocumentNumber)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El numero de documento es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationEmpty)
                .MaximumLength(20).WithMessage("El numero de documento no debe exceder los 20 caracteres.")
                .WithErrorCode(ErrorCodes.ValidationLength)
                .Matches(@"^[a-zA-Z0-9\-]+$")
                .WithMessage("El numero de documento contiene caracteres invalidos.")
                .WithErrorCode(ErrorCodes.ValidationCharacterInvalid);

            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El email es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationEmpty)
                .MaximumLength(150).WithMessage("El email no debe exceder los 150 caracteres.")
                .WithErrorCode(ErrorCodes.ValidationLength)
                .EmailAddress().WithMessage("El formato del email es invalido.")
                .WithErrorCode(ErrorCodes.ValidationCharacterInvalid);

            RuleFor(x => x.Phone)
                .MaximumLength(20).WithMessage("El telefono no debe exceder los 20 caracteres.")
                .WithErrorCode(ErrorCodes.ValidationLength)
                .Matches(@"^[0-9\s+\-()]*$")
                .WithMessage("El formato del telefono es invalido.")
                .WithErrorCode(ErrorCodes.ValidationCharacterInvalid)
                .When(x => !string.IsNullOrEmpty(x.Phone));

            RuleFor(x => x.Mobile)
                .MaximumLength(20).WithMessage("El movil no debe exceder los 20 caracteres.")
                .WithErrorCode(ErrorCodes.ValidationLength)
                .Matches(@"^[0-9\s+\-()]*$")
                .WithMessage("El formato del movil es invalido.")
                .WithErrorCode(ErrorCodes.ValidationCharacterInvalid)
                .When(x => !string.IsNullOrEmpty(x.Mobile));

            RuleFor(x => x.Address)
              .MaximumLength(300).WithMessage("La direccion no debe exceder los 300 caracteres.")
              .WithErrorCode(ErrorCodes.ValidationLength)
              .Matches(@"^[a-zA-Z0-9\s\-,.#áéíóúÁÉÍÓÚñÑ()\/]+$")
              .WithMessage("La direccion contiene caracteres invalidos.")
              .WithErrorCode(ErrorCodes.ValidationCharacterInvalid)
              .When(x => !string.IsNullOrEmpty(x.Address));
        }
    }
}
