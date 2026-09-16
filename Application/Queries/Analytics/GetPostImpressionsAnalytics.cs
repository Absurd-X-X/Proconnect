using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.Queries.Analytics
{
    public class GetPostImpressionsAnalytics
    {
        public record GetPostImpressionsAnalyticsQuery(
            Guid UserId,
            DateRangePreset Preset,
            DateTime? CustomStart,
            DateTime? CustomEnd
        ) : IRequest<Result<GetPostImpressionsAnalyticsResponse>>;

        public class GetPostImpressionsAnalyticsHandler(
            IPostRepository postRepository,
            IAnalyticsEventRepository analyticsEventRepository)
            : IRequestHandler<GetPostImpressionsAnalyticsQuery, Result<GetPostImpressionsAnalyticsResponse>>
        {
            public async Task<Result<GetPostImpressionsAnalyticsResponse>> Handle(
                GetPostImpressionsAnalyticsQuery request,
                CancellationToken cancellationToken)
            {
                var (start, end) = ResolveRange(request.Preset, request.CustomStart, request.CustomEnd);
                var span = end - start;
                var prevStart = start - span;
                var prevEnd = start;

                var postIds = await postRepository.GetPostIdsByUserAsync(request.UserId);

                var totalImpressions = await analyticsEventRepository.GetCountForSubjectsAsync(
                    AnalyticsEventType.PostImpression, postIds, start, end);

                var prevTotalImpressions = await analyticsEventRepository.GetCountForSubjectsAsync(
                    AnalyticsEventType.PostImpression, postIds, prevStart, prevEnd);

                var trend = await analyticsEventRepository.GetTrendForSubjectsAsync(
                    AnalyticsEventType.PostImpression, postIds, start, end);

                var referrerBreakdown = await analyticsEventRepository.GetReferrerBreakdownForSubjectsAsync(
                    AnalyticsEventType.PostImpression, postIds, start, end);

                var impressionsByPost = await analyticsEventRepository.GetCountsGroupedBySubjectAsync(
                    AnalyticsEventType.PostImpression, postIds, start, end);

                var contentByPostId = await postRepository.GetContentExcerptsByIdsAsync(
                    impressionsByPost.Keys.ToList());

                var topPosts = impressionsByPost
                    .Select(kv => new PostImpressionCountResponse(
                        kv.Key,
                        contentByPostId.TryGetValue(kv.Key, out var content)
                            ? (content.Length > 80 ? content[..80] + "…" : content)
                            : "(deleted post)",
                        kv.Value))
                    .OrderByDescending(x => x.ImpressionCount)
                    .Take(10)
                    .ToList();

                var response = new GetPostImpressionsAnalyticsResponse(
                    TotalImpressions: totalImpressions,
                    ImpressionsGrowthPercent: PercentChange(totalImpressions, prevTotalImpressions),
                    ImpressionsTrend: trend.Select(x => new TrendPointResponse(x.Date, x.Count)).ToList(),
                    ReferrerBreakdown: new ReferrerBreakdownResponse(
                        Direct: referrerBreakdown.TryGetValue(ReferrerSource.Direct, out var d) ? d : 0,
                        Search: referrerBreakdown.TryGetValue(ReferrerSource.Search, out var s) ? s : 0,
                        Network: referrerBreakdown.TryGetValue(ReferrerSource.Network, out var n) ? n : 0,
                        ExternalLink: referrerBreakdown.TryGetValue(ReferrerSource.ExternalLink, out var e) ? e : 0,
                        Other: referrerBreakdown.TryGetValue(ReferrerSource.Other, out var o) ? o : 0),
                    TopPostsByImpressions: topPosts);

                return Result<GetPostImpressionsAnalyticsResponse>.Success(response, "Post impression analytics retrieved successfully");
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

    public record GetPostImpressionsAnalyticsResponse(
        int TotalImpressions,
        double ImpressionsGrowthPercent,
        List<TrendPointResponse> ImpressionsTrend,
        ReferrerBreakdownResponse ReferrerBreakdown,
        List<PostImpressionCountResponse> TopPostsByImpressions);

    public record PostImpressionCountResponse(Guid PostId, string ContentExcerpt, int ImpressionCount);
}