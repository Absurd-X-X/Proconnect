using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.Queries.Analytics
{
    public class GetProfessionalViewsAnalytics
    {
        public record GetProfessionalViewsAnalyticsQuery(
            Guid ProfessionalProfileId,
            DateRangePreset Preset,
            DateTime? CustomStart,
            DateTime? CustomEnd
        ) : IRequest<Result<GetProfessionalViewsAnalyticsResponse>>;

        public class GetProfessionalViewsAnalyticsHandler(IAnalyticsEventRepository analyticsEventRepository)
            : IRequestHandler<GetProfessionalViewsAnalyticsQuery, Result<GetProfessionalViewsAnalyticsResponse>>
        {
            public async Task<Result<GetProfessionalViewsAnalyticsResponse>> Handle(
                GetProfessionalViewsAnalyticsQuery request,
                CancellationToken cancellationToken)
            {
                var (start, end) = ResolveRange(request.Preset, request.CustomStart, request.CustomEnd);
                var span = end - start;
                var prevStart = start - span;
                var prevEnd = start;

                var subjectId = request.ProfessionalProfileId;

                var totalViews = await analyticsEventRepository.GetCountAsync(
                    AnalyticsEventType.ProfileView, subjectId, start, end);

                var prevTotalViews = await analyticsEventRepository.GetCountAsync(
                    AnalyticsEventType.ProfileView, subjectId, prevStart, prevEnd);

                var trend = await analyticsEventRepository.GetTrendAsync(
                    AnalyticsEventType.ProfileView, subjectId, start, end);

                var referrerBreakdown = await analyticsEventRepository.GetReferrerBreakdownAsync(
                    subjectId, AnalyticsEventType.ProfileView, start, end);

                var response = new GetProfessionalViewsAnalyticsResponse(
                    TotalViews: totalViews,
                    ViewsGrowthPercent: PercentChange(totalViews, prevTotalViews),
                    ViewsTrend: trend.Select(x => new TrendPointResponse(x.Date, x.Count)).ToList(),
                    ReferrerBreakdown: new ReferrerBreakdownResponse(
                        Direct: referrerBreakdown.TryGetValue(ReferrerSource.Direct, out var d) ? d : 0,
                        Search: referrerBreakdown.TryGetValue(ReferrerSource.Search, out var s) ? s : 0,
                        Network: referrerBreakdown.TryGetValue(ReferrerSource.Network, out var n) ? n : 0,
                        ExternalLink: referrerBreakdown.TryGetValue(ReferrerSource.ExternalLink, out var e) ? e : 0,
                        Other: referrerBreakdown.TryGetValue(ReferrerSource.Other, out var o) ? o : 0));

                return Result<GetProfessionalViewsAnalyticsResponse>.Success(response, "Profile view analytics retrieved successfully");
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

    public record GetProfessionalViewsAnalyticsResponse(
        int TotalViews,
        double ViewsGrowthPercent,
        List<TrendPointResponse> ViewsTrend,
        ReferrerBreakdownResponse ReferrerBreakdown);

    public record ReferrerBreakdownResponse(
        int Direct, int Search, int Network, int ExternalLink, int Other);
}