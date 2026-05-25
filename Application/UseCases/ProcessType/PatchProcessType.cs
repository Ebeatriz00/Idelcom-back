using Application.DTOs.ProcessType;
using Core.Interfaces;
using FluentValidation;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppValidationException = Application.Exceptions.ValidationException;


namespace Application.UseCases.ProcessType
{
    public class PatchProcessType
    {
        private readonly IProcessTypeReporsitory _repository;
        private readonly IValidator<ProcessTypeStatusToggleDto> _validator;

        public PatchProcessType(IProcessTypeReporsitory repository, IValidator<ProcessTypeStatusToggleDto> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<GlobalResponse> ExecuteAsync(ProcessTypeStatusToggleDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                .ToList();
                throw new AppValidationException(errores);
            }

            var updated = await _repository.PatchStatusAsync(dto.ProcessTypeId, dto.Status, dto.UsersBy, dto.BusinessId);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                ? "Estado actualizado correctamente."
                : "No se pudo actualizar el estado."
            };
        }
    }
}
