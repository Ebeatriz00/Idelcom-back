using Application.DTOs.Observations;
using Application.Services.RealTime;
using AutoMapper;
using Core.Interfaces;
using Core.Interfaces.Notifications;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Observations
{
    public class UpdateObservationDate
    {
        private readonly IObservationsRepository _repository;
        private readonly IMapper _mapper;
        private readonly INotificationPush _push;

        public UpdateObservationDate(IObservationsRepository repository, IMapper mapper, INotificationPush push)
        {
            _repository = repository;
            _mapper = mapper;
            _push = push;
        }

        public async Task<GlobalResponse> ExecuteAsync(ObservationsDateUpdateDto dto)
        {

            var entity = _mapper.Map<Core.Entities.Observations>(dto);
            var updated = await _repository.UpdateDueDateAsync(entity);
            await RealtimeEvents.InvalidateBusinessAsync(
                _push,
                dto.BusinessId,
                "opportunities",
                "preSales"
            );

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                    ? "Fecha actualizada correctamente."
                    : "Error al actualizar la fecha."
            };
        }
    }
}
