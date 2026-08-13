using Host.Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static Application.Commands.Authentication.ChangePassword;
using static Application.Commands.Authentication.ForgotPassword;
using static Application.Commands.Authentication.Login;
using static Application.Commands.Authentication.Register;
using static Application.Commands.Authentication.ResendVerification;
using static Application.Commands.Authentication.ResetPassword;
using static Application.Commands.Authentication.SetupAccount;
using static Application.Commands.Authentication.VerifyEmail;

namespace Web.Controllers.Authentication
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController(IMediator mediator) : ControllerBase
    {

        [HttpPost("register")]
        public async Task<IActionResult> Register(
            [FromBody] RegisterCommand command)
        {
            var response = await mediator.Send(command);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail(
            [FromQuery] VerifyEmailCommand command)
        {
            var response = await mediator.Send(command);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginCommand command)
        
        {
            var response = await mediator.Send(command);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(
            [FromBody] ForgotPasswordCommand command)
        {
            var response = await mediator.Send(command);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(
            [FromBody] ResetPasswordCommand command)
        {
            var response = await mediator.Send(command);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("resend-verification")]
        public async Task<IActionResult> ResendVerification(
            [FromQuery] ResendVerificationCommand command)
        {
            var response = await mediator.Send(command);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("setup-account")]

        public async Task<IActionResult> SetupAccount(
            [FromBody] SetupAccountCommand command)
        {
            var response = await mediator.Send(command);

            if (!response.Status)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("change-password")]

        public async Task<IActionResult> ForgotPassword(ChangePasswordCommand command)
        {
            var result = await mediator.Send(command with { UserId = ClaimsHelper.GetUserId(User) });

            if (!result.Status)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
