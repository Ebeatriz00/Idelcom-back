using Application.Contracts;
using Application.DTOs.Quotation;
using ClosedXML.Excel;

namespace Application.Services.Excel
{
    public class QuotationExcelValidator : IQuotationExcelValidator
    {
        public QuotationExcelValidationResponseDto Validate(IXLWorkbook workbook)
        {
            return new QuotationExcelValidationResponseDto
            {
                IsValid = true
            };
        }
    }
}
