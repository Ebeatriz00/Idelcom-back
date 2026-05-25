using Application.DTOs.DocumentType;
using AppValidationException = Application.Exceptions.ValidationException;

using Core.Interfaces;
using FluentValidation;
using SharedKernel;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Application.UseCases.DocumentType;

public class PatchDocumentTypeStatus
{
    private readonly IDocumentTypeRepository _repository;
    private readonly IValidator<DocumentTypeStatusToggleDto> _validator;

    public PatchDocumentTypeStatus(IDocumentTypeRepository repository, IValidator<DocumentTypeStatusToggleDto> validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task<GlobalResponse> ExecuteAsync(DocumentTypeStatusToggleDto dto)
    {
        var validation = await _validator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            var errores = validation.Errors
            .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
            .ToList();

            throw new AppValidationException(errores);

        }

        var updated = await _repository.PatchStatusAsync(dto.DocumentTypeId, dto.Status, dto.UsersBy, dto.BusinessId);

        return new GlobalResponse
        {
            Status = updated ? 1 : 0,
            Message = updated
            ? "Estado del tipo de documento actualizado correctamente."
            : "No se pudo actualizar el estado del tipo de documento."
        };
    }
}
