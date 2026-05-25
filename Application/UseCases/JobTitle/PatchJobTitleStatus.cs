using Application.DTOs.JobTitle;
using AutoMapper;
using Core.Interfaces;
using FluentValidation;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;
using AppValidationException = Application.Exceptions.ValidationException;

namespace Application.UseCases.JobTitle
{

    public class PatchJobTitleStatus
    {
        private readonly IJobTitleRepository _repository;
        private readonly IValidator<JobTitleStatusToggleDto> _validator;

        public PatchJobTitleStatus(IJobTitleRepository repository, IValidator<JobTitleStatusToggleDto> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<GlobalResponse> ExecuteAsync(JobTitleStatusToggleDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                                .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                                .ToList();
                throw new AppValidationException(errores);
            }

            var updated = await _repository.PatchStatusAsync(dto.JobTitleId, dto.Status, dto.Usersby, dto.BusinessId);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                    ? "Estado del cargo actualizado correctamente."
                    : "No se pudo actualizar el estado del cargo."
            };
        }
    }
}
