using Application.DTOs.SsomaHomologationPersonnelDocument;
using AutoMapper;
using Core.Interfaces.Ssoma;

namespace Application.UseCases.SsomaHomologationPersonnelDocument
{
    public class GetByIdSsomaHomologationPersonnelDocument(
        ISsomaHomologationPersonnelDocumentRepository repository,
        IMapper mapper)
    {
        private readonly ISsomaHomologationPersonnelDocumentRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<SsomaHomologationPersonnelDocumentByIdDto?> ExecuteAsync(long ssomaHomologationPersonnelDocumentId, long businessId)
        {
            var entity = await _repository.GetByIdAsync(ssomaHomologationPersonnelDocumentId, businessId);
            return _mapper.Map<SsomaHomologationPersonnelDocumentByIdDto?>(entity);
        }
    }
}
