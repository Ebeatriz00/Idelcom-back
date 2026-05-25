using Application.DTOs.FileTracking;
using FluentValidation;
using SharedKernel.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.FileTracking
{
    public class FileTrackingProjectCreateValidator : AbstractValidator<FileTrackingProjectCreateDto>
    {
        public FileTrackingProjectCreateValidator()
        {
            RuleFor(x => x.BusinessId)
               .GreaterThan(0).WithMessage("El negocio es obligatorio.")
               .WithErrorCode(ErrorCodes.ValidationCharacterNegative);

            RuleFor(x => x.FileUrl)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El archivo es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationEmpty);
        }
    }
}
