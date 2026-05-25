using Application.DTOs.Opportunities;
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
    public class UpdateClientsOpportunities
    {
        private readonly IOpportunitiesRepository _repository;
        private readonly IValidator<OpportunitiesClientsUpdateDto> _validator;
        private readonly IMapper _mapper;

        public UpdateClientsOpportunities(IOpportunitiesRepository repository, IValidator<OpportunitiesClientsUpdateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }
         
        public async Task<GlobalResponse> ExecuteAsync(OpportunitiesClientsUpdateDto dto) {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                                        .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                                        .ToList();
                throw new AppValidationException(errores);
            }
            var entity = _mapper.Map<Core.Entities.Opportunities>(dto);
            var updated =  await _repository.UpdateClientsAsync(entity);
            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                    ? "Oportunidad del cliente actualizada correctamente."
                    : "Error al actualizar la oportunidad del cliente."
            };
        }
    }
}
