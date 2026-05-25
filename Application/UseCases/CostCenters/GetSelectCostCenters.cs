using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.CostCenters
{
    public class GetSelectCostCenters
    {
        private readonly ICostCentersRepository _repository;
        private readonly IMapper _mapper;

        public GetSelectCostCenters(ICostCentersRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PagedSelect<OptionItem>> ExecuteAsync(long businessId, string? search, int page, int pageSize)
        {
            var entities = await _repository.GetCostCentersForSelectAsync(businessId, search, page, pageSize);
            return _mapper.Map<PagedSelect<OptionItem>>(entities);
        }
    }
}
