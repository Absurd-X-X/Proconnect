using Application.Common.Dtos;
using Application.Common.Repositories;
using MediatR;

namespace Application.Commands.Social
{
    public class DeletePost
    {
        public record DeletePostCommand(Guid UserId, Guid PostId) : IRequest<Result<string>>;

        public class DeletePostHandler(
            IPostRepository postRepository,
            IUnitOfWork unitOfWork) : IRequestHandler<DeletePostCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(DeletePostCommand request, CancellationToken cancellationToken)
            {
                var post = await postRepository.GetByIdAsync(request.PostId);

                if (post is null)
                {
                    return Result<string>.Failure("Post not found");
                }

                if (post.UserId != request.UserId)
                {
                    return Result<string>.Failure("You are not authorized to delete this post");
                }

                post.IsDeleted = true;
                post.DateUpdated = DateTime.UtcNow;
                postRepository.Update(post);
                await unitOfWork.SaveAsync();

                return Result<string>.Success(string.Empty, "Post deleted successfully");
            }
        }
    }
}