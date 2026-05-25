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
    public class CreateConceptGroups
    {
        private readonly IConceptGroupsRepository _repository;
        private readonly IValidator<ConceptGroupsCreateDto> _validator;
        private readonly IMapper _mapper;

        public CreateConceptGroups(IConceptGroupsRepository repository, IValidator<ConceptGroupsCreateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(ConceptGroupsCreateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                                      .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                                      .ToList();
                throw new AppValidationException(errores);
            }

            var yaExiste = await _repository.ExistsAsync(dto.Description, dto.BusinessId);
            if (yaExiste)
                throw new DuplicateEntryException("El grupo de conceptos ya existe para este negocio.");

            var entity = _mapper.Map<Core.Entities.ConceptGroups>(dto);

            await _repository.AddAsync(entity);

            return new GlobalResponse
            {
                Status = 1,
                Message = "Grupo de conceptos creado exitosamente.",
            };
        }
    }
}
