using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.Queries.Analytics
{
    public class GetProfileDemographicsAnalytics
    {
        public record GetProfileDemographicsAnalyticsQuery(
            Guid ProfessionalProfileId,
            DateRangePreset Preset,
            DateTime? CustomStart,
            DateTime? CustomEnd
        ) : IRequest<Result<GetProfileDemographicsAnalyticsResponse>>;

        public class GetProfileDemographicsAnalyticsHandler(IAnalyticsEventRepository analyticsEventRepository)
            : IRequestHandler<GetProfileDemographicsAnalyticsQuery, Result<GetProfileDemographicsAnalyticsResponse>>
        {
            public async Task<Result<GetProfileDemographicsAnalyticsResponse>> Handle(
                GetProfileDemographicsAnalyticsQuery request,
                CancellationToken cancellationToken)
            {
                var (start, end) = ResolveRange(request.Preset, request.CustomStart, request.CustomEnd);

                var viewers = await analyticsEventRepository.GetViewerContextsAsync(
                    AnalyticsEventType.ProfileView, request.ProfessionalProfileId, start, end);

                var totalViews = viewers.Count;
                var anonymousCount = viewers.Count(v => v.ActorUserId is null);
                var recruiterCount = viewers.Count(v => v.HasRecruiterProfile);
                var professionalCount = viewers.Count(v => !v.HasRecruiterProfile && v.HasProfessionalProfile);
                var otherCount = totalViews - anonymousCount - recruiterCount - professionalCount;

                var industryBreakdown = viewers
                    .Where(v => v.Industry != null)
                    .GroupBy(v => v.Industry!)
                    .Select(g => new DemographicBucketResponse(g.Key, g.Count()))
                    .OrderByDescending(x => x.Count)
                    .ToList();

                var industryUnknownCount = totalViews - industryBreakdown.Sum(x => x.Count);
                if (industryUnknownCount > 0)
                    industryBreakdown.Add(new DemographicBucketResponse("Individual / Unknown", industryUnknownCount));

                var companySizeBreakdown = viewers
                    .Where(v => v.CompanySize != null)
                    .GroupBy(v => v.CompanySize!)
                    .Select(g => new DemographicBucketResponse(g.Key, g.Count()))
                    .OrderByDescending(x => x.Count)
                    .ToList();

                var companySizeUnknownCount = totalViews - companySizeBreakdown.Sum(x => x.Count);
                if (companySizeUnknownCount > 0)
                    companySizeBreakdown.Add(new DemographicBucketResponse("Individual / Unknown", companySizeUnknownCount));

                var response = new GetProfileDemographicsAnalyticsResponse(
                    TotalViews: totalViews,
                    ViewerTypeBreakdown: new ViewerTypeBreakdownResponse(
                        Professional: professionalCount,
                        Recruiter: recruiterCount,
                        Anonymous: anonymousCount,
                        Other: otherCount),
                    IndustryBreakdown: industryBreakdown,
                    CompanySizeBreakdown: companySizeBreakdown,
                    SeniorityAvailable: false);

                return Result<GetProfileDemographicsAnalyticsResponse>.Success(response, "Demographics retrieved successfully");
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

    public record GetProfileDemographicsAnalyticsResponse(
        int TotalViews,
        ViewerTypeBreakdownResponse ViewerTypeBreakdown,
        List<DemographicBucketResponse> IndustryBreakdown,
        List<DemographicBucketResponse> CompanySizeBreakdown,
        bool SeniorityAvailable);

    public record ViewerTypeBreakdownResponse(int Professional, int Recruiter, int Anonymous, int Other);

    public record DemographicBucketResponse(string Label, int Count);
}