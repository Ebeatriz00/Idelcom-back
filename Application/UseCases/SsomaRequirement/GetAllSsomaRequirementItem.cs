using Application.DTOs.SsomaRequirement;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces.Ssoma;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.UseCases.SsomaRequirement
{
    public class GetAllSsomaRequirementItem(ISsomaRequirementRepository repository, IMapper mapper)
    {
        private readonly ISsomaRequirementRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<PagedResult<SsomaRequirementItemDto>> ExecuteAsync(long businessId,  int page, int pageSize, string? search)
        {
            var pagedResult = await _repository.GetAllItemAsync( businessId,  page, pageSize, search);
            
            return new PagedResult<SsomaRequirementItemDto>
            {
                Items = _mapper.Map<IReadOnlyList<SsomaRequirementItemDto>>(pagedResult.Items),
                Page = pagedResult.Page,
                PageSize = pagedResult.PageSize,
                Total = pagedResult.Total
            };
        }
    }
}
