using Application.Common.Dtos;
using Application.Common.Repositories;
using MediatR;

namespace Application.Commands.Posts
{
    public class DeleteComment
    {
        public record DeleteCommentCommand(Guid UserId, Guid CommentId) : IRequest<Result<string>>;

        public class DeleteCommentHandler(
            ICommentRepository commentRepository,
            IUnitOfWork unitOfWork) : IRequestHandler<DeleteCommentCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
            {
                var comment = await commentRepository.GetByIdAsync(request.CommentId);

                if (comment is null)
                {
                    return Result<string>.Failure("Comment not found");
                }

                if (comment.UserId != request.UserId)
                {
                    return Result<string>.Failure("You are not authorized to delete this comment");
                }

                commentRepository.Delete(comment);
                await unitOfWork.SaveAsync();

                return Result<string>.Success(string.Empty, "Comment deleted");
            }
        }
    }
}