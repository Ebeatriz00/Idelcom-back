using Application.DTOs.ApiPeru;
using FluentValidation;
using SharedKernel.Constants;

namespace Application.Validators.ApiPeru
{
    public class ApiPeruRucLookupRequestValidator : AbstractValidator<ApiPeruRucLookupRequestDto>
    {
        public ApiPeruRucLookupRequestValidator()
        {
            RuleFor(x => x.Ruc)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("El RUC es obligatorio.")
                .WithErrorCode(ErrorCodes.ValidationEmpty)
                .Matches(@"^\d{11}$")
                .WithMessage("El RUC debe tener 11 dígitos.")
                .WithErrorCode(ErrorCodes.ValidationCharacterInvalid);
        }
    }
}
