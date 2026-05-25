using Application.DTOs.PMVis;
using Core.Interfaces;
using FluentValidation;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppValidationException = Application.Exceptions.ValidationException;


namespace Application.UseCases.PMVis
{
    public class PatchPMVisStatus
    {
        private readonly IPMVisRepository _repository;
        private readonly IValidator<PMVisStatusToggleDto> _validator;

        public PatchPMVisStatus(IPMVisRepository repository, IValidator<PMVisStatusToggleDto> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<GlobalResponse> ExecuteAsync(PMVisStatusToggleDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors.Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage)).ToList();
                throw new AppValidationException(errores);
            }

            var updated = await _repository.PatchStatusAsync(dto.PMVisId, dto.Status, dto.UsersBy, dto.BusinessId);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                ? "Estado de la visita actualizado correctamente."
                : "No se pudo actualizar el estado de la visita."
            };
        }
    }
}
