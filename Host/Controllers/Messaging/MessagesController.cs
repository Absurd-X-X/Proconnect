using Application.Commands.Messaging;
using Application.Common.Pagenation;
using Application.Queries.Messaging;
using Host.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Application.Commands.Messaging.AddParticipant;
using static Application.Commands.Messaging.CreateGroupConversation;
using static Application.Commands.Messaging.HideConversation;
using static Application.Commands.Messaging.LeaveConversation;
using static Application.Commands.Messaging.MarkConversationRead;
using static Application.Commands.Messaging.MuteConversation;
using static Application.Commands.Messaging.PinConversation;
using static Application.Commands.Messaging.SendMessage;
using static Application.Commands.Messaging.StartConversation;
using static Application.Commands.Messaging.UnhideConversation;
using static Application.Commands.Messaging.UnmuteConversation;
using static Application.Commands.Messaging.UnpinConversation;
using static Application.Queries.Messaging.GetConversationMessages;
using static Application.Queries.Messaging.GetConversationParticipants;
using static Application.Queries.Messaging.GetMyConversations;

namespace Host.Controllers.Messaging
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController(IMediator mediator) : ControllerBase
    {
        [Authorize]
        [HttpPost("start-conversation")]
        public async Task<IActionResult> StartConversation([FromQuery] Guid recipientId)
        {
            var command = new StartConversationCommand(ClaimsHelper.GetUserId(User), recipientId);

            var response = await mediator.Send(command);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [Authorize]
        [HttpPost("create-group")]
        public async Task<IActionResult> CreateGroup(CreateGroupRequest request)
        {
            var command = new CreateGroupConversationCommand(ClaimsHelper.GetUserId(User), request.Title, request.ParticipantIds);

            var response = await mediator.Send(command);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [Authorize]
        [HttpPost("add-participant")]
        public async Task<IActionResult> AddParticipant([FromQuery] Guid conversationId, [FromQuery] Guid newParticipantId)
        {
            var command = new AddParticipantCommand(ClaimsHelper.GetUserId(User), conversationId, newParticipantId);

            var response = await mediator.Send(command);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [Authorize]
        [HttpPost("leave-conversation")]
        public async Task<IActionResult> LeaveConversation([FromQuery] Guid conversationId)
        {
            var command = new LeaveConversationCommand(ClaimsHelper.GetUserId(User), conversationId);

            var response = await mediator.Send(command);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [Authorize]
        [HttpPost("send-message")]
        public async Task<IActionResult> SendMessage([FromForm] SendMessageRequest request)
        {
            var command = new SendMessageCommand(
                ClaimsHelper.GetUserId(User),
                request.ConversationId,
                request.Content,
                request.Attachments);

            var response = await mediator.Send(command);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [Authorize]
        [HttpPost("mark-read")]
        public async Task<IActionResult> MarkRead([FromQuery] Guid conversationId)
        {
            var command = new MarkConversationReadCommand(ClaimsHelper.GetUserId(User), conversationId);

            var response = await mediator.Send(command);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [Authorize]
        [HttpGet("conversations")]
        public async Task<IActionResult> GetMyConversations(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] bool usePaging = true)
        {
            var pageRequest = new PageRequest { PageNumber = pageNumber, PageSize = pageSize };

            var response = await mediator.Send(
                new GetMyConversationsQuery(ClaimsHelper.GetUserId(User), pageRequest, usePaging));

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [Authorize]
        [HttpGet("messages")]
        public async Task<IActionResult> GetMessages(
            [FromQuery] Guid conversationId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 30,
            [FromQuery] bool usePaging = true)
        {
            var pageRequest = new PageRequest { PageNumber = pageNumber, PageSize = pageSize };

            var response = await mediator.Send(
                new GetConversationMessagesQuery(ClaimsHelper.GetUserId(User), conversationId, pageRequest, usePaging));

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [Authorize]
        [HttpGet("participants")]
        public async Task<IActionResult> GetParticipants([FromQuery] Guid conversationId)
        {
            var response = await mediator.Send(
                new GetConversationParticipantsQuery(ClaimsHelper.GetUserId(User), conversationId));

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [Authorize]
        [HttpPost("pin")]
        public async Task<IActionResult> Pin([FromQuery] Guid conversationId)
        {
            var response = await mediator.Send(new PinConversationCommand(ClaimsHelper.GetUserId(User), conversationId));
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [Authorize]
        [HttpPost("unpin")]
        public async Task<IActionResult> Unpin([FromQuery] Guid conversationId)
        {
            var response = await mediator.Send(new UnpinConversationCommand(ClaimsHelper.GetUserId(User), conversationId));
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [Authorize]
        [HttpPost("mute")]
        public async Task<IActionResult> Mute([FromQuery] Guid conversationId)
        {
            var response = await mediator.Send(new MuteConversationCommand(ClaimsHelper.GetUserId(User), conversationId));
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [Authorize]
        [HttpPost("unmute")]
        public async Task<IActionResult> Unmute([FromQuery] Guid conversationId)
        {
            var response = await mediator.Send(new UnmuteConversationCommand(ClaimsHelper.GetUserId(User), conversationId));
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [Authorize]
        [HttpPost("hide")]
        public async Task<IActionResult> Hide([FromQuery] Guid conversationId)
        {
            var response = await mediator.Send(new HideConversationCommand(ClaimsHelper.GetUserId(User), conversationId));
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [Authorize]
        [HttpPost("unhide")]
        public async Task<IActionResult> Unhide([FromQuery] Guid conversationId)
        {
            var response = await mediator.Send(new UnhideConversationCommand(ClaimsHelper.GetUserId(User), conversationId));
            return response.Status ? Ok(response) : BadRequest(response);
        }

        public class CreateGroupRequest
        {
            public string Title { get; set; } = default!;
            public List<Guid> ParticipantIds { get; set; } = new();
        }

        public class SendMessageRequest
        {
            public Guid ConversationId { get; set; }
            public string? Content { get; set; }
            public List<IFormFile>? Attachments { get; set; }
        }
    }
}