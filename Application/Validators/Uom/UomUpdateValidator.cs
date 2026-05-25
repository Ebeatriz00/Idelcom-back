using Application.DTOs.Uom;
using FluentValidation;
using SharedKernel.Constants;
using SharedKernel.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.Uom
{
        public class UomUpdateValidator : AbstractValidator<UomUpdateDto>
        {
            public UomUpdateValidator()
            {
                RuleFor(x => x.UomId)
                    .GreaterThan(0)
                    .WithMessage("ID inválido.")
                    .WithErrorCode(ErrorCodes.Conflict);

                RuleFor(x => x.CodeSunat)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty()
                    .WithMessage("El código SUNAT es obligatorio.")
                    .WithErrorCode(ErrorCodes.ValidationEmpty)

                    .MaximumLength(15)
                    .WithMessage("El código SUNAT no debe exceder los 15 caracteres.")
                    .WithErrorCode(ErrorCodes.ValidationLength)

                    .Matches(@"^[a-zA-Z0-9\s\-,.áéíóúÁÉÍÓÚñÑ()]+$")
                    .WithMessage("El código SUNAT tiene caracteres especiales.")
                    .WithErrorCode(ErrorCodes.ValidationCharacterInvalid)

                    .Must(InputValidationHelpers.IsSafe)
                    .WithMessage("El código SUNAT contiene caracteres peligrosos.")
                    .WithErrorCode(ErrorCodes.ValidationIllegalChar);

                RuleFor(x => x.Description)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty()
                    .WithMessage("La descripción de la unidad de medida es obligatoria.")
                    .WithErrorCode(ErrorCodes.ValidationEmpty)

                    .MaximumLength(50)
                    .WithMessage("La descripción no debe exceder los 50 caracteres.")
                    .WithErrorCode(ErrorCodes.ValidationLength)

                    .Matches(@"^[a-zA-Z0-9\s\-,.áéíóúÁÉÍÓÚñÑ()]+$")
                    .WithMessage("La descripción tiene caracteres especiales.")
                    .WithErrorCode(ErrorCodes.ValidationCharacterInvalid)

                    .Must(InputValidationHelpers.IsSafe)
                    .WithMessage("La descripción contiene caracteres peligrosos.")
                    .WithErrorCode(ErrorCodes.ValidationIllegalChar);

                RuleFor(x => x.Symbol)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty()
                    .WithMessage("El símbolo de la unidad de medida es obligatorio.")
                    .WithErrorCode(ErrorCodes.ValidationEmpty)

                    .MaximumLength(15)
                    .WithMessage("El símbolo no debe exceder los 15 caracteres.")
                    .WithErrorCode(ErrorCodes.ValidationLength)

                    .Must(InputValidationHelpers.IsSafe)
                    .WithMessage("El símbolo contiene caracteres peligrosos.")
                    .WithErrorCode(ErrorCodes.ValidationIllegalChar);
            }
        }
    }
