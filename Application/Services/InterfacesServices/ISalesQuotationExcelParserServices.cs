using Application.DTOs.Quotation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.InterfacesServices
{
    public interface ISalesQuotationExcelParserServices
    {
        SalesQuotationCreateDto Parse(Stream excelStream);
    }
}
