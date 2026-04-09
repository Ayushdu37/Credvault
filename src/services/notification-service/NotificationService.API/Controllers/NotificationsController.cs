using CredVault.Shared.Contracts.Notification.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Application.Commands.MarkAsAllRead;
using NotificationService.Application.Commands.MarkAsRead;
using NotificationService.Application.Commands.UpdatePreferences;
using NotificationService.Application.Queries.GetNotifications;
using NotificationService.Application.Queries.GetPreferences;
using NotificationService.Application.Queries.GetUnreadCount;
using System.Security.Claims;

namespace NotificationService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public NotificationsController(IMediator mediator)
            => _mediator = mediator;

        private Guid GetUserId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        /// <summary>Get notifications (paginated)</summary>
        [HttpGet]
        public async Task<IActionResult> GetNotifications(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var result = await _mediator.Send(
                new GetNotificationsQuery(GetUserId(), page, pageSize));
            return Ok(result);
        }

        /// <summary>Get unread notification count</summary>
        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var result = await _mediator.Send(
                new GetUnreadCountQuery(GetUserId()));
            return Ok(result);
        }

        /// <summary>Mark a notification as read</summary>
        [HttpPatch("{id:guid}/read")]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            var result = await _mediator.Send(
                new MarkAsReadCommand(GetUserId(), id));
            return result.Success ? Ok(result) : NotFound(result);
        }


        /// <summary>Mark all notifications as read</summary>
        [HttpPatch("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var result = await _mediator.Send(
                new MarkAllAsReadCommand(GetUserId()));
            return Ok(result);
        }

        /// <summary>Get notification preferences</summary>
        [HttpGet("preferences")]
        public async Task<IActionResult> GetPreferences()
        {
            var result = await _mediator.Send(
                new GetPreferencesQuery(GetUserId()));
            return Ok(result);
        }

        /// <summary>Update notification preferences</summary>
        [HttpPut("preferences")]
        public async Task<IActionResult> UpdatePreferences(
            [FromBody] UpdatePreferencesRequest request)
        {
            var command = new UpdatePreferencesCommand(
                GetUserId(), request.EmailEnabled, request.PaymentAlerts,
                request.BillReminders, request.RewardUpdates);
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
