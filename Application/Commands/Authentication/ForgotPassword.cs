using Application.Common.Dtos;
using Application.Common.Repositories;
using Application.Contract.Settings;
using Application.Services.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace Application.Commands.Authentication
{
    public class ForgotPassword
    {
        public record ForgotPasswordCommand(string Email) : IRequest<Result<string>>;


        public class ForgotPasswordHAndler(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            IOptions<AppSettings> appSettings,
            IEmailService emailServices,
            IHttpContextAccessor httpContextAccessor,
            IAuditLogRepository auditLogRepository
            ) : IRequestHandler<ForgotPasswordCommand, Result<string>>
        {
            private readonly AppSettings settings = appSettings.Value;
            public async Task<Result<string>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
            {

                var user = await userRepository.GetByEmailAsync(request.Email);

                if (user is null)
                    return Result<string>.Failure("If this email exists, a reset code has been sent");

                var token = RandomNumberGenerator
                .GetInt32(100000, 999999)
                .ToString();

                user.PasswordResetToken = token;
                user.DateModified = DateTime.UtcNow;
                user.PasswordResetTokenExpiry = DateTime.UtcNow.AddMinutes(5);



                string? ipAddress = httpContextAccessor
                .HttpContext?
                .Connection
                .RemoteIpAddress?
                .ToString();




                var audit = new AuditLog
                {
                    Action = "Reset Password",
                    Description = $"Password reset request initiated from IP: {ipAddress}",
                    UserId = user.Id,
                    CreatedBy = user.Id.ToString()
                };

                await auditLogRepository.AddAsync(audit);

                await unitOfWork.SaveAsync();

                var resetLink = $"{settings.FrontendUrl.TrimEnd('/')}/reset-password.html?email={Uri.EscapeDataString(user.Email)}";

                await emailServices.SendEmailAsync(
                    new EmailRequest
                    {
                        To = user.Email,
                        Subject = "Reset Your Password",
                        Body = EmailTemplates.ForgotPasswordEmail(user.UserName, token, resetLink)
                    });

                return Result<string>.Success("You will shortly recieve a reset code if this email exists", "sent");

            }
        }
    }
}
