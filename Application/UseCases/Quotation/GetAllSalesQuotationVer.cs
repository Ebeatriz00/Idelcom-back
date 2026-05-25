using Application.DTOs.Quotation;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Quotation
{
    public class GetAllSalesQuotationVer
    {
        private readonly ISalesQuotationRepository _repository;
        private readonly IMapper _mapper;

        public GetAllSalesQuotationVer(ISalesQuotationRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<PagedResult<SalesQuotationVerResponseDto>> ExecuteAsync(string quotationId, long businessId, string? search, int page, int pageSize, string? verDesc, long? workerId, long? workerResponsibles)
        {
            var entities = await _repository.GetAllVerAsync(
        quotationId, businessId, search, page, pageSize, verDesc, workerId, workerResponsibles);

            return new PagedResult<SalesQuotationVerResponseDto>
            {
                Items = entities.Items.Select(_mapper.Map<SalesQuotationVerResponseDto>).ToList(),
                Page = entities.Page,
                PageSize = entities.PageSize,
                Total = entities.Total
            };
        }
    }
}
