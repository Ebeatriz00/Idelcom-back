using Application.DTOs.LeadsQualifications;
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

namespace Application.UseCases.LeadsQualifications
{
    public class UpdateLeadsQualifications
    {
        private readonly ILeadsQualificationsRepository _repository;
        private readonly IValidator<LeadsQualificationsUpdateDto> _validator;
        private readonly IMapper _mapper;

        public UpdateLeadsQualifications(ILeadsQualificationsRepository repository, IValidator<LeadsQualificationsUpdateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(LeadsQualificationsUpdateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors.Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage)).ToList();
                throw new AppValidationException(errores);
            }

            if (await _repository.ExistsAsync(dto.Description, dto.BusinessId, dto.LeadsQualificationsId))
                throw new DuplicateEntryException("La calificación de lead ya existe para este negocio.");

            var entity = _mapper.Map<Core.Entities.LeadsQualifications>(dto);
            var updated = await _repository.UpdateAsync(entity);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated 
                ? "Calificación de lead actualizada correctamente." 
                : "Error al actualizar la calificación de lead."
            };
        }
    }
}
