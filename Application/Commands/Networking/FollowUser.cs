using Application.Common.Dtos;
using Application.Common.Repositories;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Commands.Networking
{
    public class FollowUser
    {
        public record FollowUserCommand(Guid FollowerId, Guid FollowingId) : IRequest<Result<string>>;

        public class FollowUserHandler(
            IUserFollowRepository followRepository,
            IUserRepository userRepository,
            INotificationService notificationService,
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

                var follower = await userRepository.GetByIdAsync(request.FollowerId);
                var followerName = follower is not null ? $"{follower.FirstName} {follower.LastName}".Trim() : "Someone";

                await notificationService.SendNotificationAsync(
                    recipientUserId: request.FollowingId,
                    actorUserId: request.FollowerId,
                    actorName: followerName,
                    actorAvatarUrl: follower?.ProfilePictureUrl,
                    title: "New follower",
                    message: $"{followerName} started following you",
                    type: NotificationType.Follow,
                    sourceEntityType: null,
                    sourceEntityId: null,
                    actionUrl: $"/profile.html?userId={request.FollowerId}",
                    createdBy: request.FollowerId.ToString());

                return Result<string>.Success(string.Empty, "You are now following this user");
            }
        }
    }
}