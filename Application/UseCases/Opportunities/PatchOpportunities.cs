using Application.DTOs.Opportunities;
using Application.DTOs.PaymentMethod;
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

namespace Application.UseCases.Opportunities
{
    public class PatchOpportunities
    {
        private readonly IOpportunitiesRepository _repository;
        private readonly IValidator<OpportunitiesStatusToggleDto> _validator;
        private readonly IMapper _mapper;

        public PatchOpportunities(IOpportunitiesRepository repository, IValidator<OpportunitiesStatusToggleDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }
        public async Task<GlobalResponse> ExecuteAsync(OpportunitiesStatusToggleDto dto)
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
