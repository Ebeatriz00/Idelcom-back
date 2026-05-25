using Application.DTOs.CommercialParameters;
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

namespace Application.UseCases.CommercialParameters
{
    public class UpdateCommercialParameters
    {
        private readonly ICommercialParametersRepository _repository;
        private readonly IValidator<UpdateCommercialParametersDto> _validator;
        private readonly IMapper _mapper;

        public UpdateCommercialParameters(ICommercialParametersRepository repository, IValidator<UpdateCommercialParametersDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }
        public async Task<GlobalResponse> ExecuteAsync(UpdateCommercialParametersDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                    .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                    .ToList();
                throw new AppValidationException(errores);
            }
            var entity = _mapper.Map<Core.Entities.CommercialParameters>(dto);
            var updated = await _repository.UpdateAsync(entity);
            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                    ? "Parámetros comerciales actualizados correctamente."
                    : "Error al actualizar los parámetros comerciales."
            };
        }
    }
}
