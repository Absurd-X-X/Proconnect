using Application.Common.Dtos;
using Application.Common.Repositories;
using MediatR;

namespace Application.Queries.Social
{
    public class GetPostById
    {
        public record GetPostByIdQuery(Guid PostId, Guid CurrentUserId) : IRequest<Result<PostFeedItemResponse>>;

        public class GetPostByIdHandler(
            IPostRepository postRepository,
            IPostLikeRepository postLikeRepository) : IRequestHandler<GetPostByIdQuery, Result<PostFeedItemResponse>>
        {
            public async Task<Result<PostFeedItemResponse>> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
            {
                var post = await postRepository.GetByIdAsync(request.PostId);

                if (post is null)
                {
                    return Result<PostFeedItemResponse>.Failure("Post not found");
                }

                var reactionCounts = await postLikeRepository.GetCountsByReactionTypeAsync(post.Id);
                var myReaction = await postLikeRepository.GetByPostAndUserAsync(post.Id, request.CurrentUserId);
                var commentsCount = await postRepository.GetCommentsCountAsync(post.Id);
                var sharesCount = await postRepository.GetSharesCountAsync(post.Id);

                SharedOriginalPostResponse? original = null;

                if (post.OriginalPost is not null)
                {
                    original = new SharedOriginalPostResponse(
                        post.OriginalPost.Id,
                        post.OriginalPost.User.Id,
                        post.OriginalPost.User.FirstName,
                        post.OriginalPost.User.LastName,
                        post.OriginalPost.User.ProfilePictureUrl,
                        post.OriginalPost.Content,
                        post.OriginalPost.Attachments.OrderBy(a => a.DisplayOrder).Select(a => a.FileUrl).ToList(),
                        post.OriginalPost.DateCreated);
                }

                var response = new PostFeedItemResponse(
                    post.Id,
                    post.User.Id,
                    post.User.FirstName,
                    post.User.LastName,
                    post.User.ProfilePictureUrl,
                    post.Content,
                    post.Visibility,
                    post.Attachments.OrderBy(a => a.DisplayOrder).Select(a => a.FileUrl).ToList(),
                    original,
                    reactionCounts,
                    reactionCounts.Values.Sum(),
                    myReaction?.ReactionType,
                    commentsCount,
                    sharesCount,
                    post.DateCreated);

                return Result<PostFeedItemResponse>.Success(response, "Post retrieved successfully");
            }
        }
    }
}