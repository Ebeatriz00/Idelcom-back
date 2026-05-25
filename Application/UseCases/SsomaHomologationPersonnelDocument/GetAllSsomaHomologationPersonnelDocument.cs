using Application.DTOs.SsomaHomologationPersonnelDocument;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces.Ssoma;

namespace Application.UseCases.SsomaHomologationPersonnelDocument
{
    public class GetAllSsomaHomologationPersonnelDocument(
        ISsomaHomologationPersonnelDocumentRepository repository,
        IMapper mapper,
        SsomaHomologationPersonnelDocumentBusinessRules businessRules)
    {
        private readonly ISsomaHomologationPersonnelDocumentRepository _repository = repository;
        private readonly IMapper _mapper = mapper;
        private readonly SsomaHomologationPersonnelDocumentBusinessRules _businessRules = businessRules;

        public async Task<PagedResult<SsomaHomologationPersonnelDocumentResponseDto>> ExecuteAsync(
            long businessId,
            long? homologationPersonnelId,
            int? requirementId,
            int page,
            int pageSize,
            string? search)
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : pageSize;
            pageSize = pageSize > 100 ? 100 : pageSize;
            search = string.IsNullOrWhiteSpace(search) ? null : search.Trim();

            await _businessRules.ValidateListFilterAsync(businessId, homologationPersonnelId, requirementId);

            var entity = await _repository.GetAllAsync(
                businessId,
                homologationPersonnelId,
                requirementId,
                page,
                pageSize,
                search);

            return _mapper.Map<PagedResult<SsomaHomologationPersonnelDocumentResponseDto>>(entity);
        }
    }
}
