using Application.Common.Dtos;
using Application.Common.Repositories;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Commands.Posts
{
    public class AddComment
    {
        public record AddCommentCommand(Guid UserId, Guid PostId, string Content) : IRequest<Result<CommentResponse>>;

        public class AddCommentHandler(
            IPostRepository postRepository,
            ICommentRepository commentRepository,
            IUserRepository userRepository,
            INotificationService notificationService,
            IUnitOfWork unitOfWork) : IRequestHandler<AddCommentCommand, Result<CommentResponse>>
        {
            public async Task<Result<CommentResponse>> Handle(AddCommentCommand request, CancellationToken cancellationToken)
            {
                if (string.IsNullOrWhiteSpace(request.Content))
                {
                    return Result<CommentResponse>.Failure("Comment cannot be empty");
                }

                var post = await postRepository.GetByIdAsync(request.PostId);

                if (post is null)
                {
                    return Result<CommentResponse>.Failure("Post not found");
                }

                var comment = new Comment
                {
                    PostId = request.PostId,
                    UserId = request.UserId,
                    Content = request.Content,
                    CreatedBy = request.UserId.ToString()
                };

                await commentRepository.AddAsync(comment);
                await unitOfWork.SaveAsync();

                if (post.UserId != request.UserId)
                {
                    var commenter = await userRepository.GetByIdAsync(request.UserId);
                    var commenterName = commenter is not null ? $"{commenter.FirstName} {commenter.LastName}".Trim() : "Someone";

                    await notificationService.SendNotificationAsync(
                        recipientUserId: post.UserId,
                        actorUserId: request.UserId,
                        actorName: commenterName,
                        actorAvatarUrl: commenter?.ProfilePictureUrl,
                        title: "New comment",
                        message: $"{commenterName} commented on your post",
                        type: NotificationType.Comment,
                        sourceEntityType: NotificationSourceEntityType.Post,
                        sourceEntityId: post.Id,
                        actionUrl: $"/post-detail.html?id={post.Id}",
                        createdBy: request.UserId.ToString());
                }

                return Result<CommentResponse>.Success(
                    new CommentResponse(comment.Id, comment.Content, comment.DateCreated),
                    "Comment added");
            }
        }
    }

    public record CommentResponse(Guid Id, string Content, DateTime DateCreated);
}