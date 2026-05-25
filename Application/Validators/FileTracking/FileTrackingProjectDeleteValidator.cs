using Application.DTOs.FileTracking;
using FluentValidation;
using FluentValidation.Validators;
using SharedKernel.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.FileTracking
{
    public class FileTrackingProjectDeleteValidator: AbstractValidator<FileTrackingProjectDeleteDto>
    {
        public FileTrackingProjectDeleteValidator() {
            RuleFor(x => x.ProjectToken)
                   .Cascade(CascadeMode.Stop)
                   .NotEmpty().WithMessage("El Proyecto es obligatorio.")
                   .WithErrorCode(ErrorCodes.ValidationEmpty);

            RuleFor(x => x.LinkToken)
                   .Cascade(CascadeMode.Stop)
                   .NotEmpty().WithMessage("El id es obligatorio.")
                   .WithErrorCode(ErrorCodes.ValidationEmpty);
        }


    }

}
