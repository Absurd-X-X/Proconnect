using Application.Common.Dtos;
using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Queries.Social
{
    public class GetFeed
    {
        public record GetFeedQuery(Guid UserId, FeedTab Tab, PageRequest PageRequest, bool UsePaging)
            : IRequest<Result<PageResponse<PostFeedItemResponse>>>;

        public class GetFeedHandler(
            IPostRepository postRepository,
            IPostLikeRepository postLikeRepository,
            IUserConnectionRepository connectionRepository,
            IUserFollowRepository followRepository)
            : IRequestHandler<GetFeedQuery, Result<PageResponse<PostFeedItemResponse>>>
        {
            public async Task<Result<PageResponse<PostFeedItemResponse>>> Handle(GetFeedQuery request, CancellationToken cancellationToken)
            {
                PageResponse<Post> page;

                if (request.Tab == FeedTab.Following)
                {
                    var followingIds = await followRepository.GetFollowingAsync(
                        request.UserId, new PageRequest { PageNumber = 1, PageSize = 500 }, true);

                    var followingAuthorIds = followingIds.Items.Select(f => f.FollowingId).Append(request.UserId);

                    page = await postRepository.GetFeedByAuthorIdsAsync(followingAuthorIds, request.PageRequest, request.UsePaging);
                }
                else if (request.Tab == FeedTab.Connections)
                {
                    var connectionIds = await connectionRepository.GetUserConnectionsAsync(
                        request.UserId, new PageRequest { PageNumber = 1, PageSize = 500 }, true);
                    var connectionAuthorIds = connectionIds.Items
                        .Select(c => c.SenderId == request.UserId ? c.RecieverId : c.SenderId)
                        .Append(request.UserId);
                    page = await postRepository.GetFeedByAuthorIdsAsync(connectionAuthorIds, request.PageRequest, request.UsePaging);
                }
                else
                {
                    page = await postRepository.GetAllPublicAsync(request.PageRequest, request.UsePaging);
                }

                var items = new List<PostFeedItemResponse>();

                foreach (var post in page.Items)
                {
                    items.Add(await BuildFeedItemAsync(post, request.UserId));
                }

                var response = new PageResponse<PostFeedItemResponse>
                {
                    Items = items,
                    TotalCount = page.TotalCount,
                    PageNumber = page.PageNumber,
                    PageSize = page.PageSize
                };

                return Result<PageResponse<PostFeedItemResponse>>.Success(response, "Feed retrieved successfully");
            }

            private async Task<PostFeedItemResponse> BuildFeedItemAsync(Domain.Entities.Post post, Guid currentUserId)
            {
                var reactionCounts = await postLikeRepository.GetCountsByReactionTypeAsync(post.Id);
                var myReaction = await postLikeRepository.GetByPostAndUserAsync(post.Id, currentUserId);
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
                        post.OriginalPost.Attachments
                            .OrderBy(a => a.DisplayOrder)
                            .Select(a => a.FileUrl)
                            .ToList(),
                        post.OriginalPost.DateCreated);
                }

                return new PostFeedItemResponse(
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
            }
        }
    }

    public record PostFeedItemResponse(
        Guid Id,
        Guid AuthorId,
        string AuthorFirstName,
        string AuthorLastName,
        string? AuthorProfilePictureUrl,
        string Content,
        Visibility Visibility,
        List<string> AttachmentUrls,
        SharedOriginalPostResponse? SharedPost,
        Dictionary<ReactionType, int> ReactionCounts,
        int TotalReactions,
        ReactionType? MyReaction,
        int CommentsCount,
        int SharesCount,
        DateTime DateCreated);

    public record SharedOriginalPostResponse(
        Guid Id,
        Guid AuthorId,
        string AuthorFirstName,
        string AuthorLastName,
        string? AuthorProfilePictureUrl,
        string Content,
        List<string> AttachmentUrls,
        DateTime DateCreated);
}