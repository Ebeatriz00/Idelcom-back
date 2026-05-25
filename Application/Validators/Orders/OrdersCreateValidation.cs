using Application.DTOs.Orders;
using FluentValidation;
using SharedKernel.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.Orders
{
    public class OrdersSsomaRegisterValidator : AbstractValidator<OrdersSsomaRegister>
    {
        public OrdersSsomaRegisterValidator()
        {
            RuleFor(x => x.OperationsId)
                .GreaterThan(0).WithMessage("La operación es obligatoria.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.BusinessId)
                .GreaterThan(0).WithMessage("El negocio es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.UsersBy)
                .GreaterThan(0).WithMessage("El usuario es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationCharacterNegative);
        }
    }
}
