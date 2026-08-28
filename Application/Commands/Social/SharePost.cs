using Application.Common.Dtos;
using Application.Common.Repositories;
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

                return Result<PostResponse>.Success(
                    new PostResponse(share.Id, share.Content, share.Visibility, new List<string>(), share.DateCreated),
                    "Post shared successfully");
            }
        }
    }
}