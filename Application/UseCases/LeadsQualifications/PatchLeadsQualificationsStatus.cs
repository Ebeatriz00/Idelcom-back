using Application.DTOs.LeadsQualifications;
using Core.Interfaces;
using FluentValidation;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppValidationException = Application.Exceptions.ValidationException;

namespace Application.UseCases.LeadsQualifications
{
    public class PatchLeadsQualificationsStatus
    {
        private readonly ILeadsQualificationsRepository _repository;
        private readonly IValidator<LeadsQualificationsStatusToggleDto> _validator;

        public PatchLeadsQualificationsStatus(ILeadsQualificationsRepository repository, IValidator<LeadsQualificationsStatusToggleDto> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<GlobalResponse> ExecuteAsync(LeadsQualificationsStatusToggleDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors.Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage)).ToList();
                throw new AppValidationException(errores);
            }

            var updated = await _repository.PatchStatusAsync(dto.LeadsQualificationsId, dto.Status, dto.UsersBy, dto.BusinessId);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated ? "Estado de la calificación de lead actualizado correctamente." : "No se pudo actualizar el estado de la calificación de lead."
            };
        }
    }
}
