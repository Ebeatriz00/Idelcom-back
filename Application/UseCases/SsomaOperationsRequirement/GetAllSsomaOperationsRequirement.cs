using Application.DTOs.SsomaOperationsRequirement;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces.Ssoma;

namespace Application.UseCases.SsomaOperationsRequirement
{
    public class GetAllSsomaOperationsRequirement(
        ISsomaOperationsRequirementRepository repository,
        IMapper mapper)
    {
        private readonly ISsomaOperationsRequirementRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<PagedResult<SsomaOperationsRequirementResponseDto>> ExecuteAsync(
            long businessId,
            long operationsId,
            int page,
            int pageSize,
            string? search)
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : pageSize;
            pageSize = pageSize > 100 ? 100 : pageSize;
            search = string.IsNullOrWhiteSpace(search) ? null : search.Trim();

            var entity = await _repository.GetAllAsync(businessId, operationsId, page, pageSize, search);
            return _mapper.Map<PagedResult<SsomaOperationsRequirementResponseDto>>(entity);
        }
    }
}
