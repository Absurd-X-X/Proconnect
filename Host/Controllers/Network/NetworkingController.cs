using Application.Common.Pagenation;
using Application.Queries.Connections;
using Host.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Application.Commands.Connections.CancelConnectionRequest;
using static Application.Commands.Networking.AcceptConnectionRequest;
using static Application.Commands.Networking.FollowUser;
using static Application.Commands.Networking.RejectConnectionRequest;
using static Application.Commands.Networking.RemoveConnection;
using static Application.Commands.Networking.SendConnectionRequest;
using static Application.Commands.Networking.UnfollowUser;
using static Application.Queries.Connections.GetConnectionSuggestions;
using static Application.Queries.Connections.GetMyConnections;
using static Application.Queries.Networking.GetMyFollowers;
using static Application.Queries.Networking.GetMyFollowing;
using static Application.Queries.Networking.GetReceivedConnectionRequests;
using static Application.Queries.Networking.GetSentConnectionRequests;

namespace Host.Controllers.Connections
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConnectionsController(IMediator mediator) : ControllerBase
    {
        [Authorize]
        [HttpPost("send-connection-request")]
        public async Task<IActionResult> SendConnectionRequest([FromQuery] Guid receiverId)
        {
            var command = new SendConnectionRequestCommand(ClaimsHelper.GetUserId(User), receiverId);

            var response = await mediator.Send(command);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [Authorize]
        [HttpPost("accept-connection-request")]
        public async Task<IActionResult> AcceptConnectionRequest([FromQuery] Guid connectionId)
        {
            var command = new AcceptConnectionRequestCommand(ClaimsHelper.GetUserId(User), connectionId);

            var response = await mediator.Send(command);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [Authorize]
        [HttpPost("reject-connection-request")]
        public async Task<IActionResult> RejectConnectionRequest([FromQuery] Guid connectionId)
        {
            var command = new RejectConnectionRequestCommand(ClaimsHelper.GetUserId(User), connectionId);

            var response = await mediator.Send(command);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [Authorize]
        [HttpPost("cancel-connection-request")]
        public async Task<IActionResult> CancelConnectionRequest([FromQuery] Guid connectionId)
        {
            var command = new CancelConnectionRequestCommand(ClaimsHelper.GetUserId(User), connectionId);

            var response = await mediator.Send(command);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [Authorize]
        [HttpPost("remove-connection")]
        public async Task<IActionResult> RemoveConnection([FromQuery] Guid connectionId)
        {
            var command = new RemoveConnectionCommand(ClaimsHelper.GetUserId(User), connectionId);

            var response = await mediator.Send(command);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [Authorize]
        [HttpPost("follow-user")]
        public async Task<IActionResult> FollowUser([FromQuery] Guid userId)
        {
            var command = new FollowUserCommand(ClaimsHelper.GetUserId(User), userId);

            var response = await mediator.Send(command);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [Authorize]
        [HttpPost("unfollow-user")]
        public async Task<IActionResult> UnfollowUser([FromQuery] Guid userId)
        {
            var command = new UnfollowUserCommand(ClaimsHelper.GetUserId(User), userId);

            var response = await mediator.Send(command);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [Authorize]
        [HttpGet("my-connections")]
        public async Task<IActionResult> GetMyConnections(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool usePaging = true)
        {
            var pageRequest = new PageRequest { PageNumber = pageNumber, PageSize = pageSize };

            var response = await mediator.Send(
                new GetMyConnectionsQuery(ClaimsHelper.GetUserId(User), pageRequest, usePaging));

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [Authorize]
        [HttpGet("received-requests")]
        public async Task<IActionResult> GetReceivedRequests(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool usePaging = true)
        {
            var pageRequest = new PageRequest { PageNumber = pageNumber, PageSize = pageSize };

            var response = await mediator.Send(
                new GetReceivedConnectionRequestsQuery(ClaimsHelper.GetUserId(User), pageRequest, usePaging));

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [Authorize]
        [HttpGet("sent-requests")]
        public async Task<IActionResult> GetSentRequests(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool usePaging = true)
        {
            var pageRequest = new PageRequest { PageNumber = pageNumber, PageSize = pageSize };

            var response = await mediator.Send(
                new GetSentConnectionRequestsQuery(ClaimsHelper.GetUserId(User), pageRequest, usePaging));

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [Authorize]
        [HttpGet("my-followers")]
        public async Task<IActionResult> GetMyFollowers(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool usePaging = true)
        {
            var pageRequest = new PageRequest { PageNumber = pageNumber, PageSize = pageSize };

            var response = await mediator.Send(
                new GetMyFollowersQuery(ClaimsHelper.GetUserId(User), pageRequest, usePaging));

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [Authorize]
        [HttpGet("my-following")]
        public async Task<IActionResult> GetMyFollowing(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool usePaging = true)
        {
            var pageRequest = new PageRequest { PageNumber = pageNumber, PageSize = pageSize };

            var response = await mediator.Send(
                new GetMyFollowingQuery(ClaimsHelper.GetUserId(User), pageRequest, usePaging));

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [Authorize]
        [HttpGet("suggestions")]
        public async Task<IActionResult> GetSuggestions(
            [FromQuery] SuggestionFilter filter = SuggestionFilter.All,
            [FromQuery] int maxResults = 10)
        {
            var query = new GetConnectionSuggestionsQuery(ClaimsHelper.GetUserId(User), filter, maxResults);

            var response = await mediator.Send(query);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}