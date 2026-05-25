using Application.DTOs.Periods;
using Application.Exceptions;
using Core.Interfaces;
using FluentValidation;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppValidationException = Application.Exceptions.ValidationException;


namespace Application.UseCases.Periods
{
    public class ToggleBlockPeriods
    {
        private readonly IPeriodsRepository _repository;
        private readonly IValidator<PeriodsBlockToggleDto> _validator;

        public ToggleBlockPeriods(IPeriodsRepository repository, IValidator<PeriodsBlockToggleDto> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<GlobalResponse> ExecuteAsync(PeriodsBlockToggleDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                                    .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                                    .ToList();

                throw new AppValidationException(errores);
            }

            var success = await _repository.PatchBlockStatusAsync(
                dto.PeriodsId,
                dto.IndBlock,
                dto.UsersBy,
                dto.BusinessId
            );

            if (!success)
            {
                throw new NotFoundException("Periods", dto.PeriodsId);
            }

            return new GlobalResponse
            {
                Status = 1,
                Message = dto.IndBlock ? "Período desbloqueado exitosamente." : "Período bloqueado exitosamente."
            };
        }
    }
}
