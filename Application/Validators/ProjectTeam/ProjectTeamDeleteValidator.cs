using Application.DTOs.ProjectTeam;
using FluentValidation;
using SharedKernel.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.ProjectTeam
{
    public class ProjectTeamDeleteValidator : AbstractValidator<ProjectTeamDeleteDto>
    {
        public ProjectTeamDeleteValidator()
        {
            RuleFor(x => x.BusinessId)
                 .Cascade(CascadeMode.Stop)
                 .NotEmpty().WithMessage("La oportunidad es obligatorio.")
                 .WithErrorCode(ErrorCodes.ValidationEmpty);

            RuleFor(x => x.ProjectTeamId)
                   .Cascade(CascadeMode.Stop)
                   .NotEmpty().WithMessage("El id es obligatorio.")
                   .WithErrorCode(ErrorCodes.ValidationEmpty);
        }
    }
}
