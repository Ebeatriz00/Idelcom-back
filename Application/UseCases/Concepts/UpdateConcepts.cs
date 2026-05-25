using Application.DTOs.Concepts;
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

namespace Application.UseCases.Concepts
{
    public class UpdateConcepts
    {
        private readonly IConceptsRepository _repository;
        private readonly IValidator<ConceptsUpdateDto> _validator;
        private readonly IMapper _mapper;

        public UpdateConcepts(IConceptsRepository repository, IValidator<ConceptsUpdateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(ConceptsUpdateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                                      .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                                      .ToList();
                throw new AppValidationException(errores);
            }

            if (await _repository.ExistsAsync(dto.Description, dto.BusinessId, dto.ConceptsId))
            {
                throw new DuplicateEntryException("El concepto ya existe para este negocio.");
            }

            var entity = _mapper.Map<Core.Entities.Concepts>(dto);
            var updated = await _repository.UpdateAsync(entity);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                    ? "Concepto actualizado correctamente."
                    : "Error al actualizar el concepto."
            };
        }
    }
}
