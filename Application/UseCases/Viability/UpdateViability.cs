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
    public class UpdateViability
    {
        private readonly IViabilityRepository _repository;
        private readonly IValidator<ViabilityUpdateDto> _validator;
        private readonly IMapper _mapper;

        public UpdateViability(IViabilityRepository repository, IValidator<ViabilityUpdateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(ViabilityUpdateDto dto)
        {
            return null;
        }
    }
}
