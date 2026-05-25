using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.TaxAffType
{
    public class GetSelectTaxAffType
    {
        private readonly ITaxAffTypeRepository _taxAffTypeRepository;
        private readonly IMapper _mapper;
        public GetSelectTaxAffType(ITaxAffTypeRepository taxAffTypeRepository, IMapper mapper)
        {
            _taxAffTypeRepository = taxAffTypeRepository;
            _mapper = mapper;
        }
        public async Task<PagedSelect<OptionItem>> ExecuteAsync(long businessId, string? search, int page, int pageSize)
        {
            var entities = await _taxAffTypeRepository.GetTaxAffTypeForSelectAsync(businessId, search, page, pageSize);
            return _mapper.Map<PagedSelect<OptionItem>>(entities);
        }
    }
}
