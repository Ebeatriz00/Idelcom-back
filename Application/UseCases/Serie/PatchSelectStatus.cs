using Application.DTOs.Series;
using Core.Interfaces;
using FluentValidation;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppValidationException = Application.Exceptions.ValidationException;

namespace Application.UseCases.Series
{


    public class PatchSeriesStatus
    {
        private readonly ISeriesRepository _repository;
        private readonly IValidator<SeriesStatusToggleDto> _validator;

        public PatchSeriesStatus(ISeriesRepository repository, IValidator<SeriesStatusToggleDto> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<GlobalResponse> ExecuteAsync(SeriesStatusToggleDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                                    .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                                    .ToList();
                throw new AppValidationException(errores);
            }

            var updated = await _repository.PatchStatusAsync(dto.SeriesId, dto.Status, dto.UsersBy, dto.BusinessId);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                    ? "Estado de la serie actualizado correctamente."
                    : "No se pudo actualizar el estado de la serie."
            };
        }
    }
}
