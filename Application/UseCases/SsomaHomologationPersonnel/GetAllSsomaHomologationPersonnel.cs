using Application.DTOs.SsomaHomologationPersonnel;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces.Ssoma;

namespace Application.UseCases.SsomaHomologationPersonnel
{
    public class GetAllSsomaHomologationPersonnel(
        ISsomaHomologationPersonnelRepository repository,
        IMapper mapper,
        SsomaHomologationPersonnelBusinessRules businessRules)
    {
        private readonly ISsomaHomologationPersonnelRepository _repository = repository;
        private readonly IMapper _mapper = mapper;
        private readonly SsomaHomologationPersonnelBusinessRules _businessRules = businessRules;

        public async Task<PagedResult<SsomaHomologationPersonnelResponseDto>> ExecuteAsync(
            long businessId,
            long? operationsId,
            int page,
            int pageSize,
            string? search)
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : pageSize;
            pageSize = pageSize > 100 ? 100 : pageSize;
            search = string.IsNullOrWhiteSpace(search) ? null : search.Trim();

            await _businessRules.ValidateListFilterAsync(businessId, operationsId);

            var entity = await _repository.GetAllAsync(businessId, operationsId, page, pageSize, search);
            return _mapper.Map<PagedResult<SsomaHomologationPersonnelResponseDto>>(entity);
        }
    }
}
