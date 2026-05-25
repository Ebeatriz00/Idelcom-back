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
    public class GetAllSalesQuotation
    {
        private readonly ISalesQuotationRepository _repository;
        private readonly IMapper _mapper;
        public GetAllSalesQuotation(ISalesQuotationRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<PagedResult<SalesQuotationResponseDto>> ExecuteAsync(long businessId, string? search, int page, int pageSize, long? usersId, string? verDesc, long? workerId)
        {
            var entities = await _repository.GetAllAsync(businessId, search, page, pageSize, usersId, verDesc, workerId);
            return _mapper.Map<PagedResult<SalesQuotationResponseDto>>(entities);
        }
    }
}
