using MediatR;
using Microsoft.AspNetCore.Mvc;
using static Application.Commands.Recruiter.CreateCompany;
using static Application.Commands.Recruiter.CreateRecruiterProfile;

namespace Host.Controllers.Recruiter
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecuiterController(IMediator mediator) : ControllerBase
    {
        [HttpPost("setup-recruiter-profile")]

        public async Task<IActionResult> SetupRecruiterProfile(
            [FromBody] CreateRecruiterProfileCommand command)
        {
            var result = await mediator.Send(command);

            if (result.Status)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpPost("add-company")]

        public async Task<IActionResult> AddCompany(
            [FromBody] CreateCompanyCommand command)
        {
            var result = await mediator.Send(command);

            if (!result.Status)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
