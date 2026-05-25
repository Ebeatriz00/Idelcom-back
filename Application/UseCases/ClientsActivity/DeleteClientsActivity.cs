using Application.DTOs.ClientsActivity;
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
    public class DeleteClientsActivity
    {
        private readonly IClientsActivityRepository _repository;
        private readonly IValidator<ClientsActivityDeleteDto> _validator;

        public DeleteClientsActivity(
            IClientsActivityRepository repository,
            IValidator<ClientsActivityDeleteDto> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<GlobalResponse> ExecuteAsync(ClientsActivityDeleteDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                    .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                    .ToList();
                throw new AppValidationException(errores);
            }
            await _repository.DeleteActivityAsync(dto.BusinessId, dto.ClientsActivityId);

            return new GlobalResponse
            {
                Status = 1,
                Message = "Actividad eliminada correctamente."
            };
        }
    }
}
