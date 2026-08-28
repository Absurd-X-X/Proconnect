using Application.Common.Dtos;
using Application.Common.Repositories;
using MediatR;

namespace Application.Commands.Posts
{
    public class RemoveReaction
    {
        public record RemoveReactionCommand(Guid UserId, Guid PostId) : IRequest<Result<string>>;

        public class RemoveReactionHandler(
            IPostLikeRepository postLikeRepository,
            IUnitOfWork unitOfWork) : IRequestHandler<RemoveReactionCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(RemoveReactionCommand request, CancellationToken cancellationToken)
            {
                var existing = await postLikeRepository.GetByPostAndUserAsync(request.PostId, request.UserId);

                if (existing is null || existing.IsDeleted)
                {
                    return Result<string>.Failure("You haven't reacted to this post");
                }

                existing.IsDeleted = true;
                postLikeRepository.Update(existing);
                await unitOfWork.SaveAsync();

                return Result<string>.Success(string.Empty, "Reaction removed");
            }
        }
    }
}