using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.Queries.Analytics
{
    public class GetProfessionalAnalyticsDashboard
    {
        public record GetProfessionalAnalyticsDashboardQuery(
            Guid ProfessionalProfileId,
            Guid UserId,
            DateRangePreset Preset,
            DateTime? CustomStart,
            DateTime? CustomEnd
        ) : IRequest<Result<GetProfessionalAnalyticsDashboardResponse>>;

        public class GetProfessionalAnalyticsDashboardHandler(
            IUserConnectionRepository connectionRepository,
            IUserFollowRepository followRepository,
            IPostRepository postRepository,
            IJobApplicationRepository jobApplicationRepository)
            : IRequestHandler<GetProfessionalAnalyticsDashboardQuery, Result<GetProfessionalAnalyticsDashboardResponse>>
        {
            public async Task<Result<GetProfessionalAnalyticsDashboardResponse>> Handle(
                GetProfessionalAnalyticsDashboardQuery request,
                CancellationToken cancellationToken)
            {
                var (start, end) = ResolveRange(request.Preset, request.CustomStart, request.CustomEnd);
                var span = end - start;
                var prevStart = start - span;
                var prevEnd = start;

                var userId = request.UserId;
                var profileId = request.ProfessionalProfileId;

                var connectionGrowth = await connectionRepository.GetConnectionGrowthAsync(userId, start, end);
                var prevConnectionCount = (await connectionRepository.GetConnectionGrowthAsync(userId, prevStart, prevEnd)).Sum(x => x.Count);
                var totalConnections = await connectionRepository.GetConnectionCountAsync(userId);

                var followerGrowth = await followRepository.GetFollowerGrowthAsync(userId, start, end);
                var prevFollowerCount = (await followRepository.GetFollowerGrowthAsync(userId, prevStart, prevEnd)).Sum(x => x.Count);
                var totalFollowers = await followRepository.GetFollowerCountAsync(userId);

                var reactionCount = await postRepository.GetReactionCountAsync(userId, start, end);
                var commentCount = await postRepository.GetCommentCountForUserPostsAsync(userId, start, end);
                var shareCount = await postRepository.GetShareCountForUserPostsAsync(userId, start, end);
                var engagementInRange = reactionCount + commentCount + shareCount;

                var prevReactionCount = await postRepository.GetReactionCountAsync(userId, prevStart, prevEnd);
                var prevCommentCount = await postRepository.GetCommentCountForUserPostsAsync(userId, prevStart, prevEnd);
                var prevShareCount = await postRepository.GetShareCountForUserPostsAsync(userId, prevStart, prevEnd);
                var prevEngagementInRange = prevReactionCount + prevCommentCount + prevShareCount;

                var topPosts = await postRepository.GetTopPostsByEngagementAsync(userId, start, end, 5);

                var statusCounts = await jobApplicationRepository.GetStatusCountsByProfessionalAsync(profileId);
                var applicationsInRange = await jobApplicationRepository.GetCountInRangeByProfessionalAsync(profileId, start, end);
                var prevApplicationsInRange = await jobApplicationRepository.GetCountInRangeByProfessionalAsync(profileId, prevStart, prevEnd);

                var response = new GetProfessionalAnalyticsDashboardResponse(
                    totalConnections,
                    connectionGrowth.Sum(x => x.Count),
                    PercentChange(connectionGrowth.Sum(x => x.Count), prevConnectionCount),
                    connectionGrowth.Select(x => new TrendPointResponse(x.Date, x.Count)).ToList(),

                    totalFollowers,
                    followerGrowth.Sum(x => x.Count),
                    PercentChange(followerGrowth.Sum(x => x.Count), prevFollowerCount),
                    followerGrowth.Select(x => new TrendPointResponse(x.Date, x.Count)).ToList(),

                    engagementInRange,
                    PercentChange(engagementInRange, prevEngagementInRange),
                    topPosts.Select(p => new TopPostResponse(
                        p.PostId,
                        p.Content.Length > 120 ? p.Content[..120] + "…" : p.Content,
                        p.ReactionCount,
                        p.CommentCount,
                        p.ShareCount,
                        p.DateCreated)).ToList(),

                    statusCounts.Values.Sum(),
                    applicationsInRange,
                    PercentChange(applicationsInRange, prevApplicationsInRange));

                return Result<GetProfessionalAnalyticsDashboardResponse>.Success(response, "Dashboard analytics retrieved successfully");
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

            private static double PercentChange(int current, int previous)
            {
                if (previous == 0) return current == 0 ? 0 : 100;
                return Math.Round((current - previous) / (double)previous * 100, 1);
            }
        }
    }

    public record GetProfessionalAnalyticsDashboardResponse(
        int TotalConnections,
        int NewConnectionsInRange,
        double ConnectionsGrowthPercent,
        List<TrendPointResponse> ConnectionGrowthTrend,

        int TotalFollowers,
        int NewFollowersInRange,
        double FollowersGrowthPercent,
        List<TrendPointResponse> FollowerGrowthTrend,

        int PostEngagementsInRange,
        double PostEngagementsGrowthPercent,
        List<TopPostResponse> TopPosts,

        int TotalApplications,
        int ApplicationsInRange,
        double ApplicationsGrowthPercent);

    public record TrendPointResponse(DateTime Date, int Count);

    public record TopPostResponse(
        Guid PostId,
        string ContentExcerpt,
        int ReactionCount,
        int CommentCount,
        int ShareCount,
        DateTime DateCreated);
}