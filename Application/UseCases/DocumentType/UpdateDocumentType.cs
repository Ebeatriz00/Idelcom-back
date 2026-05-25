using Application.DTOs.DocumentType;
using Application.Exceptions;
using AutoMapper;
using Core.Interfaces;
using FluentValidation;
using SharedKernel;
using SharedKernel.Constants;
using static System.Runtime.InteropServices.JavaScript.JSType;
using AppValidationException = Application.Exceptions.ValidationException;

public class UpdateDocumentType
{
    private readonly IDocumentTypeRepository _repository;
    private readonly IValidator<DocumentTypeUpdateDto> _validator;
    private readonly IMapper _mapper;

    public UpdateDocumentType(IDocumentTypeRepository repository, IValidator<DocumentTypeUpdateDto> validator, IMapper mapper)
    {
        _repository = repository;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<GlobalResponse> ExecuteAsync(DocumentTypeUpdateDto dto)
    {
        var validation = await _validator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            var errores = validation.Errors
            .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
            .ToList();

            throw new AppValidationException(errores);

        }
        if (await _repository.ExistsAsync(dto.Description, dto.CodeSunat, dto.BusinessId, dto.DocumentTypeId))
        {
            throw new DuplicateEntryException("El tipo de documento ya existe para este negocio.");

        }


        var entity = _mapper.Map<Core.Entities.DocumentType>(dto);

        var updated = await _repository.UpdateAsync(entity);
        return new GlobalResponse
        {
            Status = updated ? 1 : 0,
            Message = updated
            ? "Actualizado correctamente."
            : "Error al actualizar."
        };
    }
}
