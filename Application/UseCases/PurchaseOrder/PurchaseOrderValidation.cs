using FluentValidation;
using SharedKernel;
using AppValidationException = Application.Exceptions.ValidationException;

namespace Application.UseCases.PurchaseOrder
{
    internal static class PurchaseOrderValidation
    {
        public static async Task ValidateAsync<T>(IValidator<T> validator, T request)
        {
            var validation = await validator.ValidateAsync(request);
            if (!validation.IsValid)
            {
                var errors = validation.Errors
                    .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                    .ToList();

                throw new AppValidationException(errors);
            }
        }
    }
}
