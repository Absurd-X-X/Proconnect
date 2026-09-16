using Application.Common.Dtos;
using Application.Common.Repositories;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Commands.Posts
{
    public class ReactToPost
    {
        public record ReactToPostCommand(Guid UserId, Guid PostId, ReactionType ReactionType) : IRequest<Result<string>>;

        public class ReactToPostHandler(
            IPostLikeRepository postLikeRepository,
            IPostRepository postRepository,
            IUserRepository userRepository,
            INotificationService notificationService,
            IUnitOfWork unitOfWork) : IRequestHandler<ReactToPostCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(ReactToPostCommand request, CancellationToken cancellationToken)
            {
                var post = await postRepository.GetByIdAsync(request.PostId);

                if (post is null)
                {
                    return Result<string>.Failure("Post not found");
                }

                var existing = await postLikeRepository.GetByPostAndUserAsync(request.PostId, request.UserId);

                if (existing is not null)
                {
                    var wasDeleted = existing.IsDeleted;

                    existing.ReactionType = request.ReactionType;
                    existing.IsDeleted = false;
                    postLikeRepository.Update(existing);
                    await unitOfWork.SaveAsync();

                    if (wasDeleted && post.UserId != request.UserId)
                    {
                        await NotifyPostOwner(post, request.UserId);
                    }

                    return Result<string>.Success(string.Empty, "Reaction updated");
                }

                var reaction = new PostLike
                {
                    PostId = request.PostId,
                    UserId = request.UserId,
                    ReactionType = request.ReactionType,
                    CreatedBy = request.UserId.ToString()
                };

                await postLikeRepository.AddAsync(reaction);
                await unitOfWork.SaveAsync();

                if (post.UserId != request.UserId)
                {
                    await NotifyPostOwner(post, request.UserId);
                }

                return Result<string>.Success(string.Empty, "Reaction added");
            }

            private async Task NotifyPostOwner(Post post, Guid actorUserId)
            {
                var reactor = await userRepository.GetByIdAsync(actorUserId);
                var reactorName = reactor is not null ? $"{reactor.FirstName} {reactor.LastName}".Trim() : "Someone";

                await notificationService.SendNotificationAsync(
                    recipientUserId: post.UserId,
                    actorUserId: actorUserId,
                    actorName: reactorName,
                    actorAvatarUrl: reactor?.ProfilePictureUrl,
                    title: "New reaction",
                    message: $"{reactorName} reacted to your post",
                    type: NotificationType.Like,
                    sourceEntityType: NotificationSourceEntityType.Post,
                    sourceEntityId: post.Id,
                    actionUrl: $"/post-detail.html?id={post.Id}",
                    createdBy: actorUserId.ToString());
            }
        }
    }
}