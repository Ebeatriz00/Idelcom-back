using Application.DTOs.DocumentType;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;

namespace Application.UseCases.DocumentType;

public class GetAllDocumentTypes
{
    private readonly IDocumentTypeRepository _repository;
    private readonly IMapper _mapper;

    public GetAllDocumentTypes(IDocumentTypeRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PagedResult<DocumentTypeResponseDto>> ExecuteAsync(int businessId, string? search, int page, int pageSize, long? usersBy)
    {
        var entities = await _repository.GetAllAsync(businessId,search, page, pageSize, usersBy);
        return _mapper.Map<PagedResult<DocumentTypeResponseDto>>(entities);
    }
}
