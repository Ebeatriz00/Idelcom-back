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
    public class FileTrackingOpperDeleteValidator : AbstractValidator<FileTrackingOpperDeleteDto>
    {
        public FileTrackingOpperDeleteValidator() {

            RuleFor(x => x.OpporToken)
                   .Cascade(CascadeMode.Stop)
                   .NotEmpty().WithMessage("La oportunidad es obligatorio.")
                   .WithErrorCode(ErrorCodes.ValidationEmpty);

            RuleFor(x => x.LinkToken)
                   .Cascade(CascadeMode.Stop)
                   .NotEmpty().WithMessage("El id es obligatorio.")
                   .WithErrorCode(ErrorCodes.ValidationEmpty);
        }
    }
}
