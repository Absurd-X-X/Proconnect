using Application.Common.Dtos;
using Application.Common.Repositories;
using MediatR;

namespace Application.Commands.Posts
{
    public class UpdateComment
    {
        public record UpdateCommentCommand(Guid UserId, Guid CommentId, string Content) : IRequest<Result<string>>;

        public class UpdateCommentHandler(
            ICommentRepository commentRepository,
            IUnitOfWork unitOfWork) : IRequestHandler<UpdateCommentCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
            {
                if (string.IsNullOrWhiteSpace(request.Content))
                {
                    return Result<string>.Failure("Comment cannot be empty");
                }

                var comment = await commentRepository.GetByIdAsync(request.CommentId);

                if (comment is null)
                {
                    return Result<string>.Failure("Comment not found");
                }

                if (comment.UserId != request.UserId)
                {
                    return Result<string>.Failure("You are not authorized to edit this comment");
                }

                comment.Content = request.Content;
                comment.DateUpdated = DateTime.UtcNow;

                commentRepository.Update(comment);
                await unitOfWork.SaveAsync();

                return Result<string>.Success(string.Empty, "Comment updated");
            }
        }
    }
}