using Application.DTOs.PreSaleProyects;
using Application.Services.RealTime;
using AutoMapper;
using Core.Interfaces;
using Core.Interfaces.Notifications;
using FluentValidation;
using Org.BouncyCastle.Tsp;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppValidationException = Application.Exceptions.ValidationException;


namespace Application.UseCases.PreSaleProyects
{
    public class UpdateStatePreSaleProyects
    {
        private readonly IPreSaleProyectsRepository _repository;
        private readonly IValidator<PreSaleProjectsUpdateStateDto> _validator;
        private readonly IMapper _mapper;
        private readonly INotificationPush _push;

        public UpdateStatePreSaleProyects(IPreSaleProyectsRepository repository, IValidator<PreSaleProjectsUpdateStateDto> validator, IMapper mapper, INotificationPush push)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
            _push = push;
        }
        public async Task<GlobalResponse> ExecuteAsync(PreSaleProjectsUpdateStateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                                        .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                                        .ToList();
                throw new AppValidationException(errores);
            }
            var entity = _mapper.Map<Core.Entities.PreSaleProyects>(dto);
            var updated = await _repository.UpdateStateAsync(entity);

            await RealtimeEvents.InvalidateBusinessAsync(
                _push,
                dto.BusinessId,
                "opportunities"
            );
            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                    ? "Estado del proyecto actualizado correctamente."
                    : "Error al actualizar el estado del proyecto."
            };
        }
    }
}
