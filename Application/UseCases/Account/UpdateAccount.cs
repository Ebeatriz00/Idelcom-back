using Application.DTOs.Account;
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


namespace Application.UseCases.Account
{
    public class UpdateAccount
    {
        private readonly IAccountRepository _repository;
        private readonly IValidator<AccountUpdateDto> _validator;
        private readonly IMapper _mapper;

        public UpdateAccount(IAccountRepository repository, IValidator<AccountUpdateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(AccountUpdateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                                        .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                                        .ToList();
                throw new AppValidationException(errores);
            }

            if (await _repository.ExistsAsync(dto.Description, dto.BusinessId, dto.AccountId))
            {
                throw new DuplicateEntryException("La cuenta ya existe para este negocio.");
            }

            var entity = _mapper.Map<Core.Entities.Account>(dto);
            var updated = await _repository.UpdateAsync(entity);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                    ? "Cuenta actualizada correctamente."
                    : "Error al actualizar la cuenta."
            };
        }
    }
}
