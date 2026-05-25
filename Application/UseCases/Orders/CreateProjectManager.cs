using Application.DTOs.Orders;
using Application.Services.RealTime;
using Core.Interfaces;
using Core.Interfaces.Notifications;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Orders
{
    public class CreateProjectManager
    {
        private readonly IOrdersRepository _repository;
        private readonly INotificationPush _push;

        public CreateProjectManager(
            IOrdersRepository repository,
            INotificationPush push)
        {
            _repository = repository;
            _push = push;
        }
        public async Task<GlobalResponse> ExecuteAsync(ProjectManagerCreateDto dto)
        {
            var orderEntity = new Core.Entities.Orders
            {
                OperationsId = dto.OperationsId,
                BusinessId = dto.BusinessId,
                WorkerId = dto.WorkerId,
                UsersBy = dto.UsersBy
            };

            var result = await _repository.AddProjectManagerAsync(orderEntity);
            if (result)
            {
                await RealtimeEvents.InvalidateBusinessAsync(
                    _push,
                    dto.BusinessId,
                    "operations"
                );
            }
            return new GlobalResponse
            {
                Status = result ? 1 : 0,
                Message = result
                    ? "Gerente de Proyecto asignado correctamente."
                    : "No se pudo actualizar la asignación del Gerente de Proyecto."
            };
        }
    }
}
