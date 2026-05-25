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
    public class GetSelectVerQuo
    {
        private readonly IOpportunitiesRepository _repository;
        private readonly IMapper _mapper;

        public GetSelectVerQuo(IOpportunitiesRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PagedSelect<OptionItem>> ExecuteAsync(long businessId, string linkToken, string? search, int page, int pageSize)
        {
            var entities = await _repository.GetForQuoVerSelectAsync(businessId, linkToken, search, page, pageSize);
            return _mapper.Map<PagedSelect<OptionItem>>(entities);
        }
    }
}
