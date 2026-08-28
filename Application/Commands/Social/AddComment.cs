using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Commands.Posts
{
    public class AddComment
    {
        public record AddCommentCommand(Guid UserId, Guid PostId, string Content) : IRequest<Result<CommentResponse>>;

        public class AddCommentHandler(
            IPostRepository postRepository,
            ICommentRepository commentRepository,
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

                return Result<CommentResponse>.Success(
                    new CommentResponse(comment.Id, comment.Content, comment.DateCreated),
                    "Comment added");
            }
        }
    }

    public record CommentResponse(Guid Id, string Content, DateTime DateCreated);
}