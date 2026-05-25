using Application.DTOs.Activity;
using Application.DTOs.Tasks;
using Core.Interfaces;
using FluentValidation;
using Infrastructure.Repositories;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AppValidationException = Application.Exceptions.ValidationException;

namespace Application.UseCases.Activity
{
    public class PatchChangeActivityStateOppor
    {
        private readonly IActivityRepository _repository;
        private readonly IValidator<ActivityStateOpporDto> _validator;
        public PatchChangeActivityStateOppor(IActivityRepository repository, IValidator<ActivityStateOpporDto> validator)
        {
            _repository = repository;
            _validator = validator;
        }
        public async Task<GlobalResponse> ExecuteAsync(ActivityStateOpporDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                                        .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                                        .ToList();
                throw new AppValidationException(errores);
            }
            var updated = await _repository.PatchActivityOpporChangeStateAsync(dto.LinkToken, dto.Status, dto.UsersBy, dto.BusinessId);
            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                    ? "ha cambiado de estado."
                    : "No se pudo cambiar el estado."
            };
        }
    }
}
