using Application.Common.Dtos;
using Application.Common.Repositories;
using Application.Contract.Settings;
using Application.Services.Interfaces;
using MediatR;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace Application.Commands.Authentication
{
    public class ResendVerification
    {
        public record ResendVerificationCommand(
            string Email
            ) : IRequest<Result<string>>;

        public class ResendVerificationHandler(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            IEmailService emailService,
            IOptions<AppSettings> appSettings
            ) : IRequestHandler<ResendVerificationCommand, Result<string>>
        {

            private readonly AppSettings settings = appSettings.Value;
            public async Task<Result<string>> Handle(ResendVerificationCommand request, CancellationToken cancellationToken)
            {
                var user = await userRepository.GetByEmailAsync(request.Email);

                if (user is null)
                {
                    return Result<string>.Failure("User not found.");
                }

                if (user.IsVerified)
                {
                    return Result<string>.Failure("User is already verified.");
                }

                if (user.VerificationTokenExpiry.HasValue && user.VerificationTokenExpiry.Value > DateTime.UtcNow)
                {
                    return Result<string>.Failure("Verification token is still valid. Please check your email.");
                }

                var token =
                RandomNumberGenerator
                .GetInt32(100000, 999999)
                .ToString();

                user.VerificationToken = token;
                user.VerificationTokenExpiry = DateTime.UtcNow.AddMinutes(5);
                user.DateModified = DateTime.UtcNow;

                await unitOfWork.SaveAsync();

                var emailResult = await emailService.SendEmailAsync(
                new EmailRequest
                {
                    To = user.Email,
                    Subject = "Verify Your ProConnect Account",
                    Body = EmailTemplates.VerificationEmail(
                        user.UserName,
                        user.Email,
                        token,
                        settings.FrontendUrl)
                });


                if (!emailResult.Status)
                {
                    return Result<string>.Failure(
                        $"Please request a new code.{emailResult.Message}");
                }

                return Result<string>.Success(
                    "Verification successful. Check your email to verify your account.",
                    "");
            }
        }
    }
}
