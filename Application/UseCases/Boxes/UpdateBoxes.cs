using Application.DTOs.Boxes;
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


namespace Application.UseCases.Boxes
{
    public class UpdateBoxes
    {
        private readonly IBoxesRepository _repository;
        private readonly IValidator<BoxesUpdateDto> _validator;
        private readonly IMapper _mapper;

        public UpdateBoxes(IBoxesRepository repository, IValidator<BoxesUpdateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(BoxesUpdateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                                        .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                                        .ToList();
                throw new AppValidationException(errores);
            }

            if (await _repository.ExistsAsync(dto.Description, dto.BusinessId, dto.BoxesId))
            {
                throw new DuplicateEntryException("La caja ya existe para este negocio.");
            }

            var entity = _mapper.Map<Core.Entities.Boxes>(dto);
            var updated = await _repository.UpdateAsync(entity);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                    ? "Caja actualizada correctamente."
                    : "Error al actualizar la caja."
            };
        }
    }
}
