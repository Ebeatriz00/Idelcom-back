using Application.DTOs.Hiring;
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

namespace Application.UseCases.Hiring
{
    public class UpdateStatusHiring
    {
        private readonly IHiringRepository _repository;
        private readonly IMapper _mapper;
        private readonly INotificationPush _push;

        public UpdateStatusHiring(IHiringRepository repository, IMapper mapper, INotificationPush push)
        {
            _repository = repository;
            _mapper = mapper;
            _push = push;
        }

        public async Task<GlobalResponse> ExecuteAsync(HiringUpdateStatusDto dto)
        {
            var entity = _mapper.Map<Core.Entities.Hiring>(dto);
            await RealtimeEvents.InvalidateBusinessAsync(
                _push,
                dto.BusinessId,
                "opportunities",
                "preSales"
            );
            return await _repository.UpdateStatusAsync(entity);


        }

    }
}
