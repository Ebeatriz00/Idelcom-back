using Application.DTOs.Viability;
using Application.Exceptions;
using AutoMapper;
using Core.Interfaces;
using Core.Interfaces.Services;
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
    public class CreateViability
    {
        private readonly IViabilityRepository _repository;
        private readonly IValidator<ViabilityCreateDto> _validator;
        private readonly IMapper _mapper;
        private readonly ILinkTokenService _linkTokenService;

        public CreateViability(IViabilityRepository repository, IValidator<ViabilityCreateDto> validator, IMapper mapper, ILinkTokenService linkTokenService)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
            _linkTokenService = linkTokenService;
        }

        public async Task<GlobalResponse> ExecuteAsync(ViabilityCreateDto dto)
        {
            return null;
        }
    }
}
