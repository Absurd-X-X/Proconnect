using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.Commands.Social
{
    public class UpdatePost
    {
        public record UpdatePostCommand(Guid UserId, Guid PostId, string Content, Visibility Visibility) : IRequest<Result<string>>;

        public class UpdatePostHandler(
            IPostRepository postRepository,
            IUnitOfWork unitOfWork) : IRequestHandler<UpdatePostCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
            {
                var post = await postRepository.GetByIdAsync(request.PostId);

                if (post is null)
                {
                    return Result<string>.Failure("Post not found");
                }

                if (post.UserId != request.UserId)
                {
                    return Result<string>.Failure("You are not authorized to edit this post");
                }

                post.Content = request.Content;
                post.Visibility = request.Visibility;
                post.DateUpdated = DateTime.UtcNow;

                postRepository.Update(post);
                await unitOfWork.SaveAsync();

                return Result<string>.Success(string.Empty, "Post updated successfully");
            }
        }
    }
}