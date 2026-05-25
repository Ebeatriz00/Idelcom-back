using Application.DTOs.ConceptGroups;
using Core.Interfaces;
using FluentValidation;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppValidationException = Application.Exceptions.ValidationException;

namespace Application.UseCases.ConceptGroups
{
    public class PatchConceptGroupsStatus
    {
        private readonly IConceptGroupsRepository _repository;
        private readonly IValidator<ConceptGroupsStatusToggleDto> _validator;

        public PatchConceptGroupsStatus(IConceptGroupsRepository repository, IValidator<ConceptGroupsStatusToggleDto> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<GlobalResponse> ExecuteAsync(ConceptGroupsStatusToggleDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                                      .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                                      .ToList();
                throw new AppValidationException(errores);
            }

            var updated = await _repository.PatchStatusAsync(dto.ConceptGroupsId, dto.Status, dto.UsersBy, dto.BusinessId);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                    ? "Estado del grupo de conceptos actualizado correctamente."
                    : "No se pudo actualizar el estado del grupo de conceptos."
            };
        }
    }
}
