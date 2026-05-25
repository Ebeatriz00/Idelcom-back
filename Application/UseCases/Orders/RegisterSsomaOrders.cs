using Application.DTOs.Orders;
using Application.Exceptions;
using Application.Services.RealTime;
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


namespace Application.UseCases.Orders 
{
    public class RegisterSsomaOrder
    {
        private readonly IOrdersRepository _repository;
        private readonly INotificationPush _push;
        private readonly IValidator<OrdersSsomaRegister> _validator;

        public RegisterSsomaOrder(
            IOrdersRepository repository,
            INotificationPush push,
            IValidator<OrdersSsomaRegister> validator)
        {
            _repository = repository;
            _push = push;
            _validator = validator;
        }

        public async Task<GlobalResponse> ExecuteAsync(OrdersSsomaRegister dto)
        {
            // 1. Validación de FluentValidation
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                    .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                    .ToList();
                throw new AppValidationException(errores);
            }


            var ordersList = new List<Core.Entities.Orders>();

            if (dto.RequeredSsoma && dto.WorkerId != null && dto.WorkerId.Any())
            {
                foreach (var id in dto.WorkerId)
                {
                    ordersList.Add(new Core.Entities.Orders
                    {
                        OperationsId = dto.OperationsId,
                        BusinessId = dto.BusinessId,
                        RequeredSsoma = dto.RequeredSsoma,
                        WorkerId = id,
                        UsersBy = dto.UsersBy
                    });
                }
            }
            else
            {
                ordersList.Add(new Core.Entities.Orders
                {
                    OperationsId = dto.OperationsId,
                    BusinessId = dto.BusinessId,
                    RequeredSsoma = dto.RequeredSsoma,
                    WorkerId = null,
                    UsersBy = dto.UsersBy
                });
            }

            // 4. Tu lógica original de persistencia y notificaciones
            var result = await _repository.AddSsomaAsync(ordersList);

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
                    ? "Asignación de SSOMA actualizada correctamente."
                    : "No se pudo actualizar la asignación de SSOMA."
            };
        }
    }
}
