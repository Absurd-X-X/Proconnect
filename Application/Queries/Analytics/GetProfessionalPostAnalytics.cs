using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.Queries.Analytics
{
    public class GetProfessionalPostAnalytics
    {
        public record GetProfessionalPostAnalyticsQuery(
            Guid UserId,
            DateRangePreset Preset,
            DateTime? CustomStart,
            DateTime? CustomEnd
        ) : IRequest<Result<GetProfessionalPostAnalyticsResponse>>;

        public class GetProfessionalPostAnalyticsHandler(IPostRepository postRepository)
            : IRequestHandler<GetProfessionalPostAnalyticsQuery, Result<GetProfessionalPostAnalyticsResponse>>
        {
            public async Task<Result<GetProfessionalPostAnalyticsResponse>> Handle(
                GetProfessionalPostAnalyticsQuery request,
                CancellationToken cancellationToken)
            {
                var (start, end) = ResolveRange(request.Preset, request.CustomStart, request.CustomEnd);
                var userId = request.UserId;

                var postTrend = await postRepository.GetPostCountTrendAsync(userId, start, end);
                var reactionBreakdown = await postRepository.GetReactionBreakdownAsync(userId, start, end);
                var reactionCount = await postRepository.GetReactionCountAsync(userId, start, end);
                var commentCount = await postRepository.GetCommentCountForUserPostsAsync(userId, start, end);
                var shareCount = await postRepository.GetShareCountForUserPostsAsync(userId, start, end);
                var topPosts = await postRepository.GetTopPostsByEngagementAsync(userId, start, end, 10);

                int ReactionOf(ReactionType type) => reactionBreakdown.TryGetValue(type, out var c) ? c : 0;

                var response = new GetProfessionalPostAnalyticsResponse(
                    TotalPosts: postTrend.Sum(x => x.Count),
                    PostCountTrend: postTrend.Select(x => new TrendPointResponse(x.Date, x.Count)).ToList(),

                    TotalReactions: reactionCount,
                    TotalComments: commentCount,
                    TotalShares: shareCount,

                    ReactionBreakdown: new ReactionBreakdownResponse(
                        Like: ReactionOf(ReactionType.Like),
                        Love: ReactionOf(ReactionType.Love),
                        Celebrate: ReactionOf(ReactionType.Celebrate),
                        Support: ReactionOf(ReactionType.Support),
                        Insightful: ReactionOf(ReactionType.Insightful),
                        Funny: ReactionOf(ReactionType.Funny)),

                    TopPosts: topPosts.Select(p => new PostAnalyticsItemResponse(
                        p.PostId,
                        p.Content.Length > 120 ? p.Content[..120] + "…" : p.Content,
                        p.ReactionCount,
                        p.CommentCount,
                        p.ShareCount,
                        p.DateCreated)).ToList());

                return Result<GetProfessionalPostAnalyticsResponse>.Success(response, "Post analytics retrieved successfully");
            }

            private static (DateTime Start, DateTime End) ResolveRange(DateRangePreset preset, DateTime? customStart, DateTime? customEnd)
            {
                var now = DateTime.UtcNow;
                return preset switch
                {
                    DateRangePreset.Last7Days => (now.AddDays(-7), now),
                    DateRangePreset.Last30Days => (now.AddDays(-30), now),
                    DateRangePreset.Last90Days => (now.AddDays(-90), now),
                    DateRangePreset.LastYear => (now.AddYears(-1), now),
                    DateRangePreset.Custom => (customStart ?? now.AddDays(-30), customEnd ?? now),
                    _ => (now.AddDays(-30), now)
                };
            }
        }
    }

    public record GetProfessionalPostAnalyticsResponse(
        int TotalPosts,
        List<TrendPointResponse> PostCountTrend,
        int TotalReactions,
        int TotalComments,
        int TotalShares,
        ReactionBreakdownResponse ReactionBreakdown,
        List<PostAnalyticsItemResponse> TopPosts);

    public record ReactionBreakdownResponse(
        int Like, int Love, int Celebrate, int Support, int Insightful, int Funny);

    public record PostAnalyticsItemResponse(
        Guid PostId, string ContentExcerpt, int ReactionCount, int CommentCount, int ShareCount, DateTime DateCreated);
}