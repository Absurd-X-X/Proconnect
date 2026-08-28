using Host.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Application.Queries.Users.GetUserPublicSummary;

namespace Host.Controllers.Users
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(IMediator mediator) : ControllerBase
    {
        [Authorize]
        [HttpGet("{userId}/public-profile")]
        public async Task<IActionResult> GetPublicProfile(Guid userId)
        {
            var response = await mediator.Send(new GetUserPublicSummaryQuery(userId, ClaimsHelper.GetUserId(User)));

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}