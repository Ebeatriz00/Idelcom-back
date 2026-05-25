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

namespace Application.UseCases.ReasonRejection
{
    public class PatchReasonRejection
    {
        private readonly IReasonRejectionRepository _repository;
        private readonly IValidator<ReasonRejectionStatusToggleDto> _validator;
        private readonly IMapper _mapper;

        public PatchReasonRejection(IReasonRejectionRepository repository, IValidator<ReasonRejectionStatusToggleDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(ReasonRejectionStatusToggleDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors.Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage)).ToList();
                throw new Application.Exceptions.ValidationException(errores);
            }

            var updated = await _repository.PatchStatusAsync(dto.ReasonRejectionId, dto.Status, dto.UsersBy, dto.BusinessId);
            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated ? "Motivo de rechazo actualizado correctamente." : "No se pudo actualizar el motivo de rechazo."
            };
        }
    }
}
