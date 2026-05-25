using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Opportunities
{
    public class GetSelectOpportunities
    {
        private readonly IOpportunitiesRepository _repository;
        private readonly IMapper _mapper;

        public GetSelectOpportunities(IOpportunitiesRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PagedSelect<OptionItem>> ExecuteAsync(long businessId, long clientsId, string? search, int page, int pageSize)
        {
            var entities = await _repository.GetForSelectAsync(businessId, clientsId, search, page, pageSize);
            return _mapper.Map<PagedSelect<OptionItem>>(entities);
        }
    }
}
