using Application.Contracts;
using Application.DTOs.Quotation;
using ClosedXML.Excel;

namespace Application.UseCases.Quotation
{
    public class ValidateQuotationExcel
    {
        private readonly IQuotationExcelValidator _validator;

        public ValidateQuotationExcel(IQuotationExcelValidator validator)
        {
            _validator = validator;
        }

        public QuotationExcelValidationResponseDto Execute(Stream excelStream)
        {
            try
            {
                if (excelStream.CanSeek)
                    excelStream.Position = 0;

                using var workbook = new XLWorkbook(excelStream);
                return _validator.Validate(workbook);
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
