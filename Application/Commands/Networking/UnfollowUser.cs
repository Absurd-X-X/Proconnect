using Application.Common.Dtos;
using Application.Common.Repositories;
using MediatR;

namespace Application.Commands.Networking
{
    public class UnfollowUser
    {
        public record UnfollowUserCommand(Guid FollowerId, Guid FollowingId) : IRequest<Result<string>>;

        public class UnfollowUserHandler(
            IUserFollowRepository followRepository,
            IUnitOfWork unitOfWork) : IRequestHandler<UnfollowUserCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(UnfollowUserCommand request, CancellationToken cancellationToken)
            {
                var follow = await followRepository.GetByFollowerAndFollowingAsync(request.FollowerId, request.FollowingId);

                if (follow is null)
                {
                    return Result<string>.Failure("You are not following this user");
                }

                followRepository.Delete(follow);
                await unitOfWork.SaveAsync();

                return Result<string>.Success(string.Empty, "Unfollowed successfully");
            }
        }
    }
}