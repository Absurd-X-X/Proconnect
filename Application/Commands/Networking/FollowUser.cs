using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Commands.Networking
{
    public class FollowUser
    {
        public record FollowUserCommand(Guid FollowerId, Guid FollowingId) : IRequest<Result<string>>;

        public class FollowUserHandler(
            IUserFollowRepository followRepository,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork) : IRequestHandler<FollowUserCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(FollowUserCommand request, CancellationToken cancellationToken)
            {
                if (request.FollowerId == request.FollowingId)
                {
                    return Result<string>.Failure("You cannot follow yourself");
                }

                var targetUser = await userRepository.GetByIdAsync(request.FollowingId);

                if (targetUser is null)
                {
                    return Result<string>.Failure("User not found");
                }

                var existing = await followRepository.GetByFollowerAndFollowingAsync(request.FollowerId, request.FollowingId);

                if (existing is not null)
                {
                    return Result<string>.Failure("You are already following this user");
                }

                var follow = new UserFollow
                {
                    FollowerId = request.FollowerId,
                    FollowingId = request.FollowingId,
                    CreatedBy = request.FollowerId.ToString()
                };

                await followRepository.AddAsync(follow);
                await unitOfWork.SaveAsync();

                return Result<string>.Success(string.Empty, "You are now following this user");
            }
        }
    }
}