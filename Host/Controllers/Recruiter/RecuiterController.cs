using Application.Commands;
using Application.Commands.Recruiter;
using Application.Queries;
using Application.Queries.Recruiter;
using Domain.Entities;
using Host.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Application.Commands.ApproveRecruiter;
using static Application.Commands.RemoveRecruiter;

namespace Host.Controllers.Recruiter
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RecruiterController(ISender sender) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateCompany(
            [FromBody] CreateCompany.CreateCompanyCommand command)
        {
            var result = await sender.Send(command with { RequestingUserId = ClaimsHelper.GetUserId(User) });
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpPost("join")]
        public async Task<IActionResult> JoinCompany(
            [FromBody] JoinCompany.JoinCompanyCommand command)
        {
            var result = await sender.Send(command with { RequestingUserId = ClaimsHelper.GetUserId(User) });
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpPost("invite")]
        public async Task<IActionResult> InviteRecruiter(
            [FromBody] InviteRecruiter.InviteRecruiterCommand command)
        {
            var result = await sender.Send(command with { RequestingUserId = ClaimsHelper.GetUserId(User) });
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpPost("recruiters/approve")]
        public async Task<IActionResult> ApproveRecruiter(
            [FromBody] ApproveRecruiterCommand command)
        {
            var result = await sender.Send(command with { RequestingUserId = ClaimsHelper.GetUserId(User) });
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("recruiters/{recruiterProfileId:guid}")]
        public async Task<IActionResult> RemoveRecruiter(Guid recruiterProfileId)
        {
            var result = await sender.Send(
                new RemoveRecruiterCommand(ClaimsHelper.GetUserId(User), recruiterProfileId));
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateCompanyProfile(
            [FromBody] UpdateCompanyProfile.UpdateCompanyProfileCommand command)
        {
            var result = await sender.Send(command with { RequestingUserId = ClaimsHelper.GetUserId(User) });
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpPut("logo")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadCompanyLogo(IFormFile file)
        {
            var result = await sender.Send(
                new UploadCompanyLogo.UploadCompanyLogoCommand(ClaimsHelper.GetUserId(User), file));
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{companyId:guid}/verify")]
        public async Task<IActionResult> VerifyCompany(Guid companyId)
        {
            var result = await sender.Send(
                new VerifyCompany.VerifyCompanyCommand(ClaimsHelper.GetUserId(User), companyId));
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{companyId:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCompanyProfile(Guid companyId)
        {
            var result = await sender.Send(new GetCompanyProfile.GetCompanyProfileQuery(companyId));
            return result.Status ? Ok(result) : NotFound(result);
        }

        [HttpGet("team")]
        public async Task<IActionResult> GetTeam(
            [FromQuery] RecruiterStatus? status,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool usePaging = true)
        {
            var result = await sender.Send(
                new GetTeam.GetTeamQuery(ClaimsHelper.GetUserId(User), status, pageNumber, pageSize, usePaging));
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpGet("management-overview")]
        public async Task<IActionResult> GetCompanyManagementOverview()
        {
            var result = await sender.Send(
                new GetCompanyManagementOverview.GetCompanyManagementOverviewQuery(ClaimsHelper.GetUserId(User)));
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpGet("invitation/{code}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCompanyByInvitationCode(string code)
        {
            var result = await sender.Send(new GetCompanyByInvitationCode.GetCompanyByInvitationCodeQuery(code));
            return result.Status ? Ok(result) : NotFound(result);
        }


        [HttpGet("profile")]
        public async Task<IActionResult> GetRecruiterProfile()
        {
            var result = await sender.Send(
                new GetRecruiterProfile.GetRecruiterProfileQuery(ClaimsHelper.GetUserId(User)));
            return result.Status ? Ok(result) : BadRequest(result);
        }
    }
}