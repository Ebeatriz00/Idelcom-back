using Application.DTOs.ReasonRejection;
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



namespace Application.UseCases.ReasonRejection
{
    public class CreateReasonRejection
    {
        private readonly IReasonRejectionRepository _repository;
        private readonly IValidator<ReasonRejectionCreateDto> _validator;
        private readonly IMapper _mapper;

        public CreateReasonRejection(IReasonRejectionRepository repository, IValidator<ReasonRejectionCreateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }
        public async Task<GlobalResponse> ExecuteAsync(ReasonRejectionCreateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors.Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage)).ToList();
                throw new AppValidationException(errores);
            }
            if (await _repository.ExistsAsync(dto.Description, dto.BusinessId))
                throw new DuplicateEntryException("El motivo de rechazp ya existe para este negocio.");

            var entity = _mapper.Map<Core.Entities.ReasonRejection>(dto);
            await _repository.AddAsync(entity);
            return new GlobalResponse
            {
                Status = 1,
                Message = "El motivo de rechazo creada exitosamente."
            };
        }
    }
}
