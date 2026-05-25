using Application.DTOs.Observations;
using Application.Services.RealTime;
using AutoMapper;
using Core.Interfaces;
using Core.Interfaces.Notifications;
using Core.Interfaces.Services;
using Infrastructure.Repositories;
using Org.BouncyCastle.Tsp;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppValidationException = Application.Exceptions.ValidationException;


namespace Application.UseCases.Observations
{
    public class CreateObservations
    {
        private readonly IObservationsRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILinkTokenService _linkToken;
        private readonly INotificationPush _push;

        public CreateObservations(
            IObservationsRepository repository,
            IMapper mapper,
            ILinkTokenService linkToken,
            INotificationPush push)
        {
            _repository = repository;
            _mapper = mapper;
            _linkToken = linkToken;
            _push = push;
        }

        public async Task<GlobalResponse> ExecuteAsync(ObservationsCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.OpporToken))
                throw new AppValidationException(new List<GlobalErrorDetail> {
                    new GlobalErrorDetail("1007", "El token de la oportunidad es obligatorio.")
                });
            if (!_linkToken.TryValidate(dto.OpporToken, out var entityType, out var resourceId))
                throw new AppValidationException(new List<GlobalErrorDetail> {
                    new GlobalErrorDetail("1008", "Token inválido o expirado.")
                });

            if (entityType != "opportunity")
                throw new AppValidationException(new List<GlobalErrorDetail> {
                    new GlobalErrorDetail("1009", "Token no pertenece a una oportunidad.")
                });

            long realOpporId = Convert.ToInt64(resourceId);

            if (realOpporId == 0)
            {
                throw new AppValidationException(new List<GlobalErrorDetail> {
                    new GlobalErrorDetail("1007", "La oportunidad es obligatoria.")
                });
            }


            var entity = _mapper.Map<Core.Entities.Observations>(dto);
            if (dto.ObsReason != null && dto.ObsReason.Any())
            {
                entity.ObsReason = string.Join("\n", dto.ObsReason);
            }
            entity.OpporId = realOpporId; 
            entity.OpenedBy = dto.UsersBy;
            entity.DueSetBy = dto.UsersBy;
            entity.OpenedAt = DateTime.Now;
            entity.Status = "1";
            entity.AffectsQuotation = dto.AffectsQuotation;

            await _repository.AddAsync(new[] { entity });

            await RealtimeEvents.InvalidateBusinessAsync(
                _push,
                dto.BusinessId,
                "opportunities",
                "preSales"
            );

            return new GlobalResponse
            {
                Status = 1,
                Message = "Observación registrada exitosamente."
            };
        }
    }
}
