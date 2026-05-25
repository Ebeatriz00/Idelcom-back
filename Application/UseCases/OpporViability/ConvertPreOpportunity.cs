using Application.DTOs.OpporViability;
using Application.Services.RealTime;
using Core.Interfaces;
using Core.Interfaces.Notifications;
using DocumentFormat.OpenXml.EMMA;
using Org.BouncyCastle.Tsp;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppValidationException = Application.Exceptions.ValidationException;


namespace Application.UseCases.OpporViability
{
    public class ConvertPreOpportunity
    {
        private readonly IOpporViabilityRepository _repository;
        private readonly INotificationPush _push;

        public ConvertPreOpportunity(IOpporViabilityRepository repository, INotificationPush push)
        {
            _repository = repository;
            _push = push;
        }

        public async Task<GlobalResponse> ExecuteAsync(OpporViabilityConvertDto dto)
        {
            var opporId = long.Parse(dto.LinkToken);


            var result = await _repository.ProcessDecisionAsync(opporId, dto.BusinessId, dto.UsersBy, dto.IsApproved, dto.RejectionReason);
            await RealtimeEvents.InvalidateBusinessAsync(
                _push,
                dto.BusinessId,
                "opportunities",
                "preSales"
            );

            return new GlobalResponse
            {
                Status = result.Success ? 1 : 0,
                Message = result.Message
            };
        }
    }
}
