using Application.DTOs.Orders;
using Application.Services.RealTime;
using Core.Entities;
using Core.Interfaces;
using Core.Interfaces.Notifications;
using SharedKernel;
using System.Threading.Tasks;

namespace Application.UseCases.Orders
{
    public class RegisterQualitySupervisor
    {
        private readonly IOrdersRepository _repository;
        private readonly INotificationPush _push;

        public RegisterQualitySupervisor(
            IOrdersRepository repository,
            INotificationPush push)
        {
            _repository = repository;
            _push = push;
        }

        public async Task<GlobalResponse> ExecuteAsync(QualitySupervisorCreateDto dto)
        {
            var orderEntity = new Core.Entities.Orders
            {
                OperationsId = dto.OperationsId,
                BusinessId = dto.BusinessId,
                WorkerId = dto.WorkerId, 
                UsersBy = dto.UsersBy
            };

            var result = await _repository.AddQualitySupervisorAsync(orderEntity);
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
                    ? "Supervisor de calidad asignado correctamente."
                    : "No se pudo actualizar la asignación del supervisor de calidad."
            };
        }
    }
}