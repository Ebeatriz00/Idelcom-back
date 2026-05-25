using Application.DTOs.Opportunities;
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
    public class UpdateOpportunities
    {
        private readonly IOpportunitiesRepository _repository;
        private readonly IValidator<OpportunitiesUpdateDto> _validator;
        private readonly IMapper _mapper;

        public UpdateOpportunities(IOpportunitiesRepository repository, IValidator<OpportunitiesUpdateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }
        public async Task<GlobalResponse> ExecuteAsync(OpportunitiesUpdateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                    .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                    .ToList();
                throw new AppValidationException(errores);
            }

            if (await _repository.ExistsAsync(dto.OpporDesc, dto.BusinessId, dto.LinkToken))
                throw new DuplicateEntryException("La oportunidad ya existe para este negocio.");

            var entity = _mapper.Map<Core.Entities.Opportunities>(dto);

            
            var res = await _repository.UpdateAsync(entity);

            if (res.Status != 1 && string.IsNullOrWhiteSpace(res.Message))
                res.Message = res.Message;

            return res;
        }

    }
}
