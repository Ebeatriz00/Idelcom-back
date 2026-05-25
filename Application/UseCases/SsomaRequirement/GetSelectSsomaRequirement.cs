using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces.Ssoma;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.SsomaRequirement
{
    public class GetSelectSsomaRequirement (ISsomaRequirementRepository repository, IMapper mapper)
    {
        private readonly ISsomaRequirementRepository _repository = repository;
        private readonly IMapper _mapper = mapper;
        public async Task<PagedSelect<OptionItem>> ExecuteAsync(long businessId, int scopedId, string? search, int page, int pageSize)
        {
            var result = await _repository.GetForSelectAsync(businessId, scopedId, search, page, pageSize);
            return _mapper.Map<PagedSelect<OptionItem>>(result);
        }
    }
}
