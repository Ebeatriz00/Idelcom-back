using Application.DTOs.DocumentType;
using AutoMapper;
using Core.Interfaces;

namespace Application.UseCases.DocumentType;

public class GetByIdDocumentTypes
{
    private readonly IDocumentTypeRepository _repository;
    private readonly IMapper _mapper;

    public GetByIdDocumentTypes(IDocumentTypeRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<DocumentTypeResponseDto> ExecuteAsync(int docuemntTypeId)
    {
        var entities = await _repository.GetByIdAsync(docuemntTypeId);
        return _mapper.Map<DocumentTypeResponseDto>(entities);
    }
}
