using Application.DTOs.Uom;
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

namespace Application.UseCases.Uom
{
        public class CreateUom
        {
            private readonly IUomRepository _repository;
            private readonly IValidator<UomCreateDto> _validator;
            private readonly IMapper _mapper;

            public CreateUom(IUomRepository repository, IValidator<UomCreateDto> validator, IMapper mapper)
            {
                _repository = repository;
                _validator = validator;
                _mapper = mapper;
            }

            public async Task<GlobalResponse> ExecuteAsync(UomCreateDto dto)
            {
                // Validación de reglas con FluentValidation
                var validation = await _validator.ValidateAsync(dto);
                if (!validation.IsValid)
                {
                    var errores = validation.Errors
                        .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                        .ToList();

                    throw new AppValidationException(errores);
                }

                // Verifica si la unidad de medida ya existe
                var yaExiste = await _repository.ExistsAsync(dto.Description, dto.CodeSunat, dto.BusinessId);
                if (yaExiste)
                    throw new DuplicateEntryException("La unidad de medida ya existe para este negocio.");

                // Mapeo del DTO a la entidad
                var entity = _mapper.Map<Core.Entities.Uom>(dto);
                await _repository.AddAsync(entity);

                return new GlobalResponse
                {
                    Status = 1,
                    Message = "Unidad de medida creada exitosamente.",
                };
            }
        }
    }
