using Application.Common.Dtos;
using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Queries.Analytics
{
    public class GetJobViewsAnalytics
    {
        public record GetJobViewsAnalyticsQuery(
            Guid CallerRecruiterProfileId,
            DateRangePreset Preset,
            DateTime? CustomStart,
            DateTime? CustomEnd,
            Guid? JobId
        ) : IRequest<Result<GetJobViewsAnalyticsResponse>>;

        public class GetJobViewsAnalyticsHandler(
            IRecruiterProfileRepository recruiterProfileRepository,
            IJobRepository jobRepository,
            IAnalyticsEventRepository analyticsEventRepository)
            : IRequestHandler<GetJobViewsAnalyticsQuery, Result<GetJobViewsAnalyticsResponse>>
        {
            public async Task<Result<GetJobViewsAnalyticsResponse>> Handle(
                GetJobViewsAnalyticsQuery request,
                CancellationToken cancellationToken)
            {
                var callingRecruiter = await recruiterProfileRepository.GetByIdAsync(request.CallerRecruiterProfileId);

                if (callingRecruiter is null || callingRecruiter.Status != RecruiterStatus.Active || callingRecruiter.CompanyId is null)
                    return Result<GetJobViewsAnalyticsResponse>.Failure("You are not authorized to view this data");

                var companyId = callingRecruiter.CompanyId.Value;
                var (start, end) = ResolveRange(request.Preset, request.CustomStart, request.CustomEnd);
                var span = end - start;
                var prevStart = start - span;
                var prevEnd = start;

                if (request.JobId.HasValue)
                {
                    var subjectId = request.JobId.Value;

                    var totalViews = await analyticsEventRepository.GetCountAsync(AnalyticsEventType.JobView, subjectId, start, end);
                    var prevTotalViews = await analyticsEventRepository.GetCountAsync(AnalyticsEventType.JobView, subjectId, prevStart, prevEnd);
                    var trend = await analyticsEventRepository.GetTrendAsync(AnalyticsEventType.JobView, subjectId, start, end);
                    var referrerBreakdown = await analyticsEventRepository.GetReferrerBreakdownAsync(subjectId, AnalyticsEventType.JobView, start, end);

                    return Result<GetJobViewsAnalyticsResponse>.Success(
                        BuildResponse(totalViews, prevTotalViews, trend, referrerBreakdown, null),
                        "Job view analytics retrieved successfully");
                }
                else
                {
                    var jobIds = await jobRepository.GetJobIdsByCompanyAsync(companyId);

                    var totalViews = await analyticsEventRepository.GetCountForSubjectsAsync(AnalyticsEventType.JobView, jobIds, start, end);
                    var prevTotalViews = await analyticsEventRepository.GetCountForSubjectsAsync(AnalyticsEventType.JobView, jobIds, prevStart, prevEnd);
                    var trend = await analyticsEventRepository.GetTrendForSubjectsAsync(AnalyticsEventType.JobView, jobIds, start, end);
                    var referrerBreakdown = await analyticsEventRepository.GetReferrerBreakdownForSubjectsAsync(AnalyticsEventType.JobView, jobIds, start, end);
                    var viewsByJob = await analyticsEventRepository.GetCountsGroupedBySubjectAsync(AnalyticsEventType.JobView, jobIds, start, end);

                    var jobs = await jobRepository.SearchAsync(
                        new JobSearchFilter { CompanyId = companyId },
                        new PageRequest(),
                        false);

                    var perJob = viewsByJob
                        .Select(kv => new JobViewCountResponse(
                            kv.Key,
                            jobs.Items.FirstOrDefault(j => j.Id == kv.Key)?.Title ?? "(deleted job)",
                            kv.Value))
                        .OrderByDescending(x => x.ViewCount)
                        .Take(10)
                        .ToList();

                    return Result<GetJobViewsAnalyticsResponse>.Success(
                        BuildResponse(totalViews, prevTotalViews, trend, referrerBreakdown, perJob),
                        "Job view analytics retrieved successfully");
                }
            }

            private static GetJobViewsAnalyticsResponse BuildResponse(
                int totalViews,
                int prevTotalViews,
                List<IAnalyticsEventRepository.DateCountDto> trend,
                Dictionary<ReferrerSource, int> referrerBreakdown,
                List<JobViewCountResponse>? perJob)
            {
                return new GetJobViewsAnalyticsResponse(
                    TotalViews: totalViews,
                    ViewsGrowthPercent: PercentChange(totalViews, prevTotalViews),
                    ViewsTrend: trend.Select(x => new TrendPointResponse(x.Date, x.Count)).ToList(),
                    ReferrerBreakdown: new ReferrerBreakdownResponse(
                        Direct: referrerBreakdown.TryGetValue(ReferrerSource.Direct, out var d) ? d : 0,
                        Search: referrerBreakdown.TryGetValue(ReferrerSource.Search, out var s) ? s : 0,
                        Network: referrerBreakdown.TryGetValue(ReferrerSource.Network, out var n) ? n : 0,
                        ExternalLink: referrerBreakdown.TryGetValue(ReferrerSource.ExternalLink, out var e) ? e : 0,
                        Other: referrerBreakdown.TryGetValue(ReferrerSource.Other, out var o) ? o : 0),
                    ViewsByJob: perJob);
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

    public record GetJobViewsAnalyticsResponse(
        int TotalViews,
        double ViewsGrowthPercent,
        List<TrendPointResponse> ViewsTrend,
        ReferrerBreakdownResponse ReferrerBreakdown,
        List<JobViewCountResponse>? ViewsByJob);

    public record JobViewCountResponse(Guid JobId, string Title, int ViewCount);
}