using Application.DTOs.OpporViability;
using AutoMapper;
using Core.Interfaces;
using FluentValidation;
using Org.BouncyCastle.Tsp;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppValidationException = Application.Exceptions.ValidationException;


namespace Application.UseCases.OpporViability
{
    public class PatchOpporViability
    {
        private readonly IOpporViabilityRepository _repository;
        private readonly IValidator<OpporViabilityStatusToggleDto> _validator;
        private readonly IMapper _mapper;

        public PatchOpporViability(IOpporViabilityRepository repository, IValidator<OpporViabilityStatusToggleDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(OpporViabilityStatusToggleDto dto)
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
