using Application.DTOs.Clients;
using AutoMapper;
using Core.Entities.Email;
using Core.Interfaces;
using Core.Interfaces.Notifications;
using FluentValidation;
using SharedKernel;
using System.Linq;
using System.Threading.Tasks;
using AppValidationException = Application.Exceptions.ValidationException;

namespace Application.UseCases.Clients
{
    public class UpdateChangeSalesClients
    {
        private readonly IClientsRepository _repository;
        private readonly IValidator<ClientsUpdateChangeSalesDto> _validator;
        private readonly IMapper _mapper;
        private readonly IEmailSender _email;
        private readonly IEmailTemplateRenderer _tpl;

        public UpdateChangeSalesClients(
            IClientsRepository repository,
            IValidator<ClientsUpdateChangeSalesDto> validator,
            IMapper mapper,
            IEmailSender email,
            IEmailTemplateRenderer tpl)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
            _email = email;
            _tpl = tpl;
        }

        public async Task<GlobalResponse> ExecuteAsync(ClientsUpdateChangeSalesDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                    .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                    .ToList();
                throw new AppValidationException(errores);
            }

            var entity = _mapper.Map<Core.Entities.Clients>(dto);
            var result = await _repository.UpdateChangeSalesAsync(entity);

            return new GlobalResponse
            {
                Status = result ? 1 : 0,
                Message = result
                    ? "Actualizado correctamente."
                    : "Error al actualizar el cliente."
            };
        }
    }
}
