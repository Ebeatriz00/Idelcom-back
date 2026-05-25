using Application.DTOs.SsomaRequirement;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces.Ssoma;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.UseCases.SsomaRequirement
{
    public class GetAllSsomaRequirement(ISsomaRequirementRepository repository, IMapper mapper)
    {
        private readonly ISsomaRequirementRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<PagedResult<SsomaRequirementResponseDto>> ExecuteAsync(long businessId, int scopeId, int page, int pageSize, string? search)
        {
            var pagedResult = await _repository.GetAllAsync(businessId, scopeId, page, pageSize, search);
            
            return new PagedResult<SsomaRequirementResponseDto>
            {
                Items = _mapper.Map<IReadOnlyList<SsomaRequirementResponseDto>>(pagedResult.Items),
                Page = pagedResult.Page,
                PageSize = pagedResult.PageSize,
                Total = pagedResult.Total
            };
        }
    }
}
