using Application.DTOs.ClientsActivity;
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


namespace Application.UseCases.ClientsActivity
{
    public class CreateClientsActivity
    {
        private readonly IClientsActivityRepository _repository;
        private readonly IValidator<ClientActivityCreateDto> _validator;
        private readonly IMapper _mapper;

        public CreateClientsActivity(IClientsActivityRepository repository, IValidator<ClientActivityCreateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(ClientActivityCreateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                                        .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                                        .ToList();
                throw new AppValidationException(errores);
            }

            var entity = _mapper.Map<Core.Entities.ClientsActivity>(dto);
            var newId = await _repository.AddActivityAsync(entity);

            return new GlobalResponse
            {
                Status = 1,
                Message = "Actividad registrada correctamente."
            };
        }
    }
}
