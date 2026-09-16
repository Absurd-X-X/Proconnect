using Application.Common.Dtos;
using Application.Common.Repositories;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Commands.Social
{
    public class SharePost
    {
        public record SharePostCommand(Guid UserId, Guid OriginalPostId, string? Content) : IRequest<Result<PostResponse>>;

        public class SharePostHandler(
            IPostRepository postRepository,
            IUserRepository userRepository,
            INotificationService notificationService,
            IUnitOfWork unitOfWork) : IRequestHandler<SharePostCommand, Result<PostResponse>>
        {
            public async Task<Result<PostResponse>> Handle(SharePostCommand request, CancellationToken cancellationToken)
            {
                var postToShare = await postRepository.GetByIdAsync(request.OriginalPostId);

                if (postToShare is null)
                {
                    return Result<PostResponse>.Failure("Post not found");
                }

                var trueOriginalId = postToShare.OriginalPostId ?? postToShare.Id;
                var trueOriginalOwnerId = postToShare.OriginalPost?.UserId ?? postToShare.UserId;

                if (trueOriginalOwnerId == request.UserId)
                {
                    return Result<PostResponse>.Failure("You can't reshare your own post");
                }

                var share = new Post
                {
                    UserId = request.UserId,
                    Content = request.Content ?? string.Empty,
                    Visibility = Visibility.Public,
                    OriginalPostId = trueOriginalId,
                    CreatedBy = request.UserId.ToString()
                };

                await postRepository.AddAsync(share);
                await unitOfWork.SaveAsync();

                var sharer = await userRepository.GetByIdAsync(request.UserId);
                var sharerName = sharer is not null ? $"{sharer.FirstName} {sharer.LastName}".Trim() : "Someone";

                await notificationService.SendNotificationAsync(
                    recipientUserId: trueOriginalOwnerId,
                    actorUserId: request.UserId,
                    actorName: sharerName,
                    actorAvatarUrl: sharer?.ProfilePictureUrl,
                    title: "Your post was shared",
                    message: $"{sharerName} shared your post",
                    type: NotificationType.Share,
                    sourceEntityType: NotificationSourceEntityType.Post,
                    sourceEntityId: trueOriginalId,
                    actionUrl: $"/post-detail.html?id={share.Id}",
                    createdBy: request.UserId.ToString());

                return Result<PostResponse>.Success(
                    new PostResponse(share.Id, share.Content, share.Visibility, new List<string>(), share.DateCreated),
                    "Post shared successfully");
            }
        }
    }
}