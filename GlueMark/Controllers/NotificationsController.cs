using Application.DTOs.Notifications;
using Application.UseCases.Notifications;
using Core.Interfaces.Notifications;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly PushNotificationUseCase _pushNotification;
        private readonly ListNotifications _listNotifications;
        private readonly MarkAllRead _markAllReadNotifications;
        private readonly MarkRead _markAsReadNotification;
        private readonly AlertResolve _alertResolve;
        private readonly AlertSnooze _alertSnooze;

        public NotificationsController(PushNotificationUseCase pushNotification, ListNotifications listNotifications, MarkAllRead markAllReadNotifications, MarkRead markAsReadNotification, AlertResolve alertResolve, AlertSnooze alertSnooze)
        {
            _pushNotification = pushNotification;
            _listNotifications = listNotifications;
            _markAllReadNotifications = markAllReadNotifications;
            _markAsReadNotification = markAsReadNotification;
            _alertResolve = alertResolve;
            _alertSnooze = alertSnooze;
        }

        [HttpPost("new-comment")]
        public async Task<IActionResult> TestNewComment()
        {
            var notification = new NewComment(
                opporId: "1",
                createdBy: 1,
                businessId: 1,
                commentPreview: "Este es un comentario de prueba",
                commentId: "1",
                createdByName: "Usuario de Prueba"
            );

            await _pushNotification.HandleAsync(notification, HttpContext.RequestAborted);

            return Ok(new
            {
                message = "Notificación enviada",
                opportunityId = 1
            });
        }

        [HttpPost("state-changed")]
        public async Task<IActionResult> TestStateChanged()
        {
            var notification = new OpportunityStateChanged(
                opporId: "1",
                newState: "APPROVED",
                changedBy: 1,
               businessId: 1 
            );

            await _pushNotification.HandleAsync(notification, HttpContext.RequestAborted);

            return Ok(new
            {
                message = "Notificación de cambio de estado enviada",
                newState = "APPROVED"
            });
        }

        [HttpGet("ListNotifications")]
        public async Task<IActionResult> List(long businessId, long usersId, string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 30, CancellationToken ct = default)
        {
        
            var result = await _listNotifications.HandleAsync(businessId, usersId, search, page, pageSize, ct);
            return Ok(result);
        }
        [HttpPut("MarkAllRead")]
        public async Task<IActionResult> MarkAllRead(NotificationsMarkAllDto notificationsMarkAllDto, CancellationToken ct)
        {
            await _markAllReadNotifications.HandleAsync(notificationsMarkAllDto, ct);
            return Ok(new { ok = true });
        }
        [HttpPut("MarkRead")]
        public async Task<IActionResult> MarkRead(NotificationsMarkDto notificationsMarkDto, CancellationToken ct)
        {
            await _markAsReadNotification.HandleAsync(notificationsMarkDto, ct);
            return Ok(new { ok = true });
        }
        [HttpPut("AlertResolve")]
        public async Task<IActionResult> AlertResolve(long businessId, long usersId, long notificationId, CancellationToken ct)
        {
            await _alertResolve.HandleAsync(businessId, usersId, notificationId, ct);
            return Ok(new { ok = true });

        }
        
        [HttpPut("AlertSnooze")]
        public async Task<IActionResult> AlertResolve(long businessId, long usersId, long notificationId, DateTime snoozeUntil, string comment, CancellationToken ct)
        {
            await _alertSnooze.HandleAsync(businessId, usersId, notificationId, snoozeUntil, comment, ct);
            return Ok(new { ok = true });

        }

        [HttpPost("test-cache")]
        public async Task<IActionResult> TestCache([FromServices] INotificationPush push)
        {
            await push.PushToBusinessAsync(1,
                new { code = "CACHE_INVALIDATE", keys = new[] { "preSales" } },
                CancellationToken.None);

            return Ok();
        }
    }
}
