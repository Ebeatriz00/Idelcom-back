
using Application.DTOs.Quotation;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Quotation
{
    public class GetDetailSalesQuotation
    {
        private readonly ISalesQuotationRepository _repository;
        private readonly IMapper _mapper;

        public GetDetailSalesQuotation(ISalesQuotationRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<SalesQuotationDetailDto> ExecuteAsync(long quotationVerId, long businessId, string versionNo)
        {
            var entity = await _repository.GetDetailAsync(quotationVerId, businessId, versionNo);
            return _mapper.Map<SalesQuotationDetailDto>(entity);
        }
    }
}
