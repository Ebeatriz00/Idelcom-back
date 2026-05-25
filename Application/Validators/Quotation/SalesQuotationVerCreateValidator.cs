using Application.DTOs.Quotation;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.Quotation
{
    public class SalesQuotationVerCreateValidator : AbstractValidator<SalesQuotationVerCreateDto>
    {
        public SalesQuotationVerCreateValidator() {
            RuleFor(x => x.BusinessId)
                .GreaterThan(0)
                .WithMessage("El id del negocio es obligatorio.");

            RuleFor(x => x.OpporId)
                .GreaterThan(0)
                .WithMessage("El id de la opportunidad es obligatorio.");

            RuleFor(x => x)
                .Must(x => x.ClientsId.HasValue || !string.IsNullOrWhiteSpace(x.ClientsName))
                .WithMessage("Debe enviar id o nombre del cliente.");

            RuleFor(x => x.SubTotal).GreaterThanOrEqualTo(0);
            RuleFor(x => x.DiscountAmount).GreaterThanOrEqualTo(0);
            RuleFor(x => x.TaxAmount).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Total).GreaterThanOrEqualTo(0);

            RuleFor(x => x.UsersBy)
                .GreaterThan(0)
                .WithMessage("Usuario creador es obligatorio.");
        }
    }
}
