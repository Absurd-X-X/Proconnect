using Application.Common.Dtos;
using Application.Common.Repositories;
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
                    existing.ReactionType = request.ReactionType;
                    existing.IsDeleted = false;
                    postLikeRepository.Update(existing);
                    await unitOfWork.SaveAsync();

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

                return Result<string>.Success(string.Empty, "Reaction added");
            }
        }
    }
}