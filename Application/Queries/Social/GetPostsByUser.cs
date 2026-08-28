using Application.Common.Dtos;
using Application.Common.Pagenation;
using Application.Common.Repositories;
using MediatR;

namespace Application.Queries.Social
{
    public class GetPostsByUser
    {
        public record GetPostsByUserQuery(Guid ProfileUserId, Guid CurrentUserId, PageRequest PageRequest, bool UsePaging)
            : IRequest<Result<PageResponse<PostFeedItemResponse>>>;

        public class GetPostsByUserHandler(
            IPostRepository postRepository,
            IPostLikeRepository postLikeRepository) : IRequestHandler<GetPostsByUserQuery, Result<PageResponse<PostFeedItemResponse>>>
        {
            public async Task<Result<PageResponse<PostFeedItemResponse>>> Handle(GetPostsByUserQuery request, CancellationToken cancellationToken)
            {
                var page = await postRepository.GetByUserIdAsync(request.ProfileUserId, request.PageRequest, request.UsePaging);

                var items = new List<PostFeedItemResponse>();

                foreach (var post in page.Items)
                {
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

                    items.Add(new PostFeedItemResponse(
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
                        post.DateCreated));
                }

                var response = new PageResponse<PostFeedItemResponse>
                {
                    Items = items,
                    TotalCount = page.TotalCount,
                    PageNumber = page.PageNumber,
                    PageSize = page.PageSize
                };

                return Result<PageResponse<PostFeedItemResponse>>.Success(response, "Posts retrieved successfully");
            }
        }
    }
}