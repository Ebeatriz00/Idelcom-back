using FluentValidation;
using AppValidationException = Application.Exceptions.ValidationException;

namespace Application.UseCases.PurchaseReceipt
{
    internal static class PurchaseReceiptValidation
    {
        public static async Task ValidateAsync<T>(IValidator<T> validator, T dto)
        {
            var validation = await validator.ValidateAsync(dto);
            if (!validation.IsValid)
                throw new AppValidationException(validation.Errors
                    .Select(e => new SharedKernel.GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                    .ToList());
        }
    }
}
