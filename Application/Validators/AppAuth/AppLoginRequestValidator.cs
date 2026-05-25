using Application.DTOs.AppAuth;
using FluentValidation;

namespace Application.Validators.AppAuth
{
    public class AppLoginRequestValidator : AbstractValidator<AppLoginRequestDto>
    {
        public AppLoginRequestValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("El nombre de usuario es requerido.")
                .MaximumLength(50).WithMessage("El nombre de usuario no puede exceder los 50 caracteres.");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("La contraseña es requerida.")
                .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres.")
                .MaximumLength(20).WithMessage("La contraseña no puede exceder los 20 caracteres.");
            RuleFor(x => x.DeviceId)
                .NotEmpty().WithMessage("El identificador del dispositivo es requerido.")
                .MaximumLength(200).WithMessage("El identificador del dispositivo no puede exceder los 200 caracteres.");
            RuleFor(x => x.DeviceName)
                .NotEmpty().WithMessage("El nombre del dispositivo es requerido.")
                .MaximumLength(200).WithMessage("El nombre del dispositivo no puede exceder los 200 caracteres.");
            RuleFor(x => x.DeviceBrand)
                .NotEmpty().WithMessage("La marca del dispositivo es requerido.")
                .MaximumLength(200).WithMessage("El nombre del dispositivo no puede exceder los 200 caracteres.");
            RuleFor(x => x.DeviceVersion)
                .NotEmpty().WithMessage("La versión del dispositivo es requerido.")
                .MaximumLength(200).WithMessage("El nombre del dispositivo no puede exceder los 200 caracteres.");
            RuleFor(x => x.AppVersion)
                .NotEmpty().WithMessage("La versión del aplicativo es requerido.")
                .MaximumLength(200).WithMessage("El nombre del dispositivo no puede exceder los 200 caracteres.");
        }
    }
}
