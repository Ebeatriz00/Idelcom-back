using Application.DTOs.Quotation;
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


namespace Application.UseCases.Quotation
{
    public class CreateSalesQuotation
    {
        private readonly ISalesQuotationRepository _repository;
        private readonly IValidator<SalesQuotationCreateDto> _validator;
        private readonly IMapper _mapper;
        public CreateSalesQuotation(ISalesQuotationRepository repository, IValidator<SalesQuotationCreateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }
        public async Task<GlobalResponse> ExecuteAsync(SalesQuotationCreateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                            .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                            .ToList();
                throw new AppValidationException(errores);
            }
            var entity = _mapper.Map<Core.Entities.SalesQuotation>(dto);
            var response = await _repository.AddAsync(entity);

            return response;
        }
    }
}
