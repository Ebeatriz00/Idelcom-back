using Application.DTOs.ReasonRejection;
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


namespace Application.UseCases.ReasonRejection
{
    public class UpdateReasonRejection
    {
        private readonly IReasonRejectionRepository _repository;
        private readonly IValidator<ReasonRejectionUpdateDto> _validator;
        private readonly IMapper _mapper;
        public UpdateReasonRejection(IReasonRejectionRepository repository, IValidator<ReasonRejectionUpdateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }
        public async Task<GlobalResponse> ExecuteAsync(ReasonRejectionUpdateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors.Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage)).ToList();
                throw new AppValidationException(errores);
            }

            if (await _repository.ExistsAsync(dto.Description, dto.BusinessId, dto.ReasonRejectionId))
                throw new Application.Exceptions.DuplicateEntryException("El motivo de rechazo ya existe para este negocio.");

            var entity = _mapper.Map<Core.Entities.ReasonRejection>(dto);
            var updated = await _repository.UpdateAsync(entity);
            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated ? "Motivo de rechazo actualizado correctamente." : "No se pudo actualizar el motivo de rechazo."
            };
        }
    }
}
