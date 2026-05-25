using Application.DTOs.ConceptType;
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


namespace Application.UseCases.ConceptType
{
    public class UpdateConceptType
    {
        private readonly IConceptTypeRepository _repository;
        private readonly IValidator<ConceptTypeUpdateDto> _validator;
        private readonly IMapper _mapper;

        public UpdateConceptType(IConceptTypeRepository repository, IValidator<ConceptTypeUpdateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(ConceptTypeUpdateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                .ToList();

                throw new AppValidationException(errores);
            }
            if (await _repository.ExistsAsync(dto.Description, dto.BusinessId, dto.ConceptTypeId))
            {
                throw new DuplicateEntryException("La descripción del tipo de concepto ya existe para este negocio.");
            }

            var entity = _mapper.Map<Core.Entities.ConceptType>(dto);
            var updated = await _repository.UpdateAsync(entity);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                ? "Tipo de concepto actualizado correctamente."
                : "Error al actualizar el tipo de concepto. El ID podría ser inválido."
            };
        }
    }
}
