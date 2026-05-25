using Application.DTOs.ConceptGroups;
using Application.Exceptions;
using AutoMapper;
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
    public class UpdateConceptGroups
    {
        private readonly IConceptGroupsRepository _repository;
        private readonly IValidator<ConceptGroupsUpdateDto> _validator;
        private readonly IMapper _mapper;

        public UpdateConceptGroups(IConceptGroupsRepository repository, IValidator<ConceptGroupsUpdateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(ConceptGroupsUpdateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                                      .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                                      .ToList();
                throw new AppValidationException(errores);
            }

            if (await _repository.ExistsAsync(dto.Description, dto.BusinessId, dto.ConceptGroupsId))
            {
                throw new DuplicateEntryException("El grupo de conceptos ya existe para este negocio.");
            }

            var entity = _mapper.Map<Core.Entities.ConceptGroups>(dto);
            var updated = await _repository.UpdateAsync(entity);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                    ? "Grupo de conceptos actualizado correctamente."
                    : "Error al actualizar el grupo de conceptos."
            };
        }
    }
}
