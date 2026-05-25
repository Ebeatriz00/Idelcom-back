using Application.Services.RealTime;
using Core.Entities;
using Core.Interfaces;
using Core.Interfaces.Notifications;
using Core.Interfaces.Services;
using FluentValidation;
using global::Application.DTOs.PreSaleProyects;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Application.UseCases.PreSaleProyects
{
   

    namespace Application.UseCases.PreSaleProyects 
    {
        public class UpdateResponsiblePreSaleProject
        {
            private readonly IPreSaleProyectsRepository _repository;
            private readonly ILinkTokenService _linkTokenService;
            private readonly INotificationPush _push;


            public UpdateResponsiblePreSaleProject(
                IPreSaleProyectsRepository repository,
                ILinkTokenService linkTokenService,
                INotificationPush push)
            {
                _repository = repository;
                _linkTokenService = linkTokenService;
                _push = push;
            }

            public async Task<GlobalResponse> ExecuteAsync(PreSaleProyectsUpdateResponsibleDto dto)
            {
          
                if (!_linkTokenService.ValidateToken(dto.LinkToken, out _, out var entityType, out var resourceIdString))
                {
                    return new GlobalResponse
                    {
                        Status = 0,
                        Message = "Token inválido o expirado."
                    };
                }

                if (entityType != "opportunity")
                {
                    return new GlobalResponse
                    {
                        Status = 0,
                        Message = "El token no corresponde a una oportunidad válida."
                    };
                }

                var opporId = long.Parse(resourceIdString);

                var entity = new Core.Entities.PreSaleProyects
                {
                    OpportunityId = opporId, 
                    ResponsibleId = dto.WorkerId,
                    ProjectCategory = dto.ProjectCategory,
                    BusinessId = dto.BusinessId,
                    UsersBy = dto.UsersBy
                };

                var result = await _repository.UpdateResponsibleAsync(entity);
                await RealtimeEvents.InvalidateBusinessAsync(
                _push,
                dto.BusinessId,
                "opportunities"
            );

                return new GlobalResponse
                {
                    Status = result ? 1 : 0,
                    Message = result
                        ? "Responsable actualizado correctamente."
                        : "No se pudo actualizar el responsable."
                };
            }
        }
    }
}
