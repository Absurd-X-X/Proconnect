using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Commands.Authentication
{
    public class Logout
    {
        public record LogoutCommand(Guid UserId)
            : IRequest<Result<string>>;


        public class LogoutHandler(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            IAuditLogRepository auditLogRepository)
            : IRequestHandler<LogoutCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(
                LogoutCommand request,
                CancellationToken cancellationToken)
            {
                var user = await userRepository.GetByIdAsync(request.UserId);

                if (user is null)
                {
                    return Result<string>.Failure("User not found");
                }


                user.RefreshToken = null;
                user.RefreshTokenExpiry = null;
                user.DateModified = DateTime.UtcNow;


                userRepository.UpdateAsync(user);

                await auditLogRepository.AddAsync(new AuditLog
                {
                    UserId = user.Id,
                    Description = $"Logout: ({user.UserName})",
                    Action = "Logout",
                });

                await unitOfWork.SaveAsync();


                return Result<string>.Success(
                    "Logout successful",
                    "logged_out");
            }
        }
    }
}