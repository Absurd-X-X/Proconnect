using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Commands.Authentication
{
    public class ResetPassword
    {
        public record ResetPasswordCommand(
            string Email,
            string ResetCode,
            string NewPassword
        ) : IRequest<Result<string>>;

        public class ResetPasswordHandler(
            IPasswordHasher<User> passwordHasher,
            IUnitOfWork unitOfWork,
            IUserRepository userRepository) : IRequestHandler<ResetPasswordCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(
                ResetPasswordCommand request,
                CancellationToken cancellationToken)
            {
                var user = await userRepository.GetByEmailAsync(request.Email);

                if (user is null)
                {
                    return Result<string>.Failure("Invalid or expired reset code");
                }

                if (string.IsNullOrEmpty(user.PasswordResetToken) ||
                    user.PasswordResetToken != request.ResetCode)
                {
                    return Result<string>.Failure("Invalid or expired reset code");
                }

                if (user.PasswordResetTokenExpiry is null ||
                    user.PasswordResetTokenExpiry < DateTime.UtcNow)
                {
                    return Result<string>.Failure("Invalid or expired reset code");
                }

                var hash = passwordHasher.HashPassword(user, request.NewPassword);
                user.HashedPassword = hash;

                user.PasswordResetToken = null;
                user.PasswordResetTokenExpiry = null;
                user.DateModified = DateTime.UtcNow;

                await unitOfWork.SaveAsync();

                return Result<string>.Success("Password reset successfully", "reset");
            }
        }
    }
}