using Application.Commands.Notifications;
using Application.Common.Pagenation;
using Application.Queries.Notifications;
using Host.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Application.Queries.Notifications.GetNotifications;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationsController(IMediator mediator) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetNotifications(
            [FromQuery] string? status,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] bool usePaging = true)
        {
            var userId = ClaimsHelper.GetUserId(User);

            var query = new GetNotificationsQuery(
                userId, status, new PageRequest { PageNumber = pageNumber, PageSize = pageSize }, usePaging);

            var response = await mediator.Send(query);
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = ClaimsHelper.GetUserId(User);
            var response = await mediator.Send(new GetUnreadNotificationCount.GetUnreadNotificationCountQuery(userId));
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpPost("mark-read")]
        public async Task<IActionResult> MarkAsRead([FromQuery] Guid notificationId)
        {
            var userId = ClaimsHelper.GetUserId(User);
            var response = await mediator.Send(new MarkNotificationAsRead.MarkNotificationAsReadCommand(userId, notificationId));
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpPost("mark-all-read")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = ClaimsHelper.GetUserId(User);
            var response = await mediator.Send(new MarkAllNotificationsAsRead.MarkAllNotificationsAsReadCommand(userId));
            return response.Status ? Ok(response) : BadRequest(response);
        }
    }
}