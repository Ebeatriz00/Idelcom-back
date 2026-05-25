using Application.DTOs.Viability;
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


namespace Application.UseCases.Viability
{
    public class PatchViabilityStatus
    {
        private readonly IViabilityRepository _repository;
        private readonly IValidator<ViabilityStatusToggleDto> _validator;
        private readonly IMapper _mapper;

        public PatchViabilityStatus(IViabilityRepository repository, IValidator<ViabilityStatusToggleDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(ViabilityStatusToggleDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                .ToList();

                throw new AppValidationException(errores);
            }

            var updated = await _repository.PatchStatusAsync(dto.LinkToken, dto.Status, dto.UsersBy, dto.BusinessId);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                ? "Estado actualizado correctamente."
                : "No se pudo actualizar el estado."
            };
        }
    }
}
