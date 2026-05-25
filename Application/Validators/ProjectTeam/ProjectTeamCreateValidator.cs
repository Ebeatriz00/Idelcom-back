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
    public class ProjectTeamCreateValidator : AbstractValidator<ProjectTeamCreateDto>
    {
        public ProjectTeamCreateValidator() {

            RuleFor(x => x.BusinessId)
                   .GreaterThan(0).WithMessage("El negocio es obligatorio.")
                   .WithErrorCode(ErrorCodes.ValidationCharacterNegative);


        }
    }
}
