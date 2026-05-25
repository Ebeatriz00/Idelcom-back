using Application.DTOs.Quotation;
using ClosedXML.Excel;

namespace Application.Contracts
{
    public interface IQuotationExcelValidator
    {
        QuotationExcelValidationResponseDto Validate(IXLWorkbook workbook);
    }
}
