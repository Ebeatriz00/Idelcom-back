using Application.DTOs.Quotation;
using Application.Services.Excel;
using ClosedXML.Excel;

namespace Application.UseCases.Quotation
{
    public class ValidateQuotationExcel
    {
        public QuotationExcelValidationResponseDto Execute(Stream excelStream, string? fileName = null)
        {
            try
            {
                if (excelStream.CanSeek)
                    excelStream.Position = 0;

                var formulaErrors = ExcelFormulaErrorScanner.Scan(excelStream);
                if (formulaErrors.Count > 0)
                {
                    return new QuotationExcelValidationResponseDto
                    {
                        IsValid = false,
                        Errors = formulaErrors
                    };
                }

                if (excelStream.CanSeek)
                    excelStream.Position = 0;

                using var sanitized = ExcelSanitizer.SanitizeBrokenRefs(excelStream);
                using var workbook = new XLWorkbook(sanitized);

                return new QuotationExcelValidationResponseDto
                {
                    IsValid = true
                };
            }
            catch (Exception ex)
            {
                return new QuotationExcelValidationResponseDto
                {
                    IsValid = false,
                    Errors = new List<QuotationExcelValidationError>
                    {
                        new() { Sheet = "", Message = $"No se pudo abrir el archivo Excel: {ex.Message}" }
                    }
                };
            }
        }
    }
}
