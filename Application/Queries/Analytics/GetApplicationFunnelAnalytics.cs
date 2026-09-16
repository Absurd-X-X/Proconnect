using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Queries.Analytics
{
    public class GetApplicationFunnelAnalytics
    {
        public record GetApplicationFunnelAnalyticsQuery(
            Guid CallerRecruiterProfileId,
            DateRangePreset Preset,
            DateTime? CustomStart,
            DateTime? CustomEnd,
            Guid? JobId
        ) : IRequest<Result<GetApplicationFunnelAnalyticsResponse>>;

        public class GetApplicationFunnelAnalyticsHandler(
            IRecruiterProfileRepository recruiterProfileRepository,
            IJobApplicationRepository jobApplicationRepository)
            : IRequestHandler<GetApplicationFunnelAnalyticsQuery, Result<GetApplicationFunnelAnalyticsResponse>>
        {
            public async Task<Result<GetApplicationFunnelAnalyticsResponse>> Handle(
                GetApplicationFunnelAnalyticsQuery request,
                CancellationToken cancellationToken)
            {
                var callingRecruiter = await recruiterProfileRepository.GetByIdAsync(request.CallerRecruiterProfileId);

                if (callingRecruiter is null || callingRecruiter.Status != RecruiterStatus.Active || callingRecruiter.CompanyId is null)
                    return Result<GetApplicationFunnelAnalyticsResponse>.Failure("You are not authorized to view this data");

                var companyId = callingRecruiter.CompanyId.Value;
                var (start, end) = ResolveRange(request.Preset, request.CustomStart, request.CustomEnd);

                var filter = new JobApplicationFilter
                {
                    JobId = request.JobId,
                    AppliedAfter = start,
                    AppliedBefore = end
                };

                var counts = await jobApplicationRepository.GetStatusCountsByCompanyAsync(companyId, filter);
                var interviewStats = await jobApplicationRepository.GetInterviewStatsByCompanyAsync(companyId, start, end);

                int Count(JobStatus status) => counts.TryGetValue(status, out var c) ? c : 0;
                var total = counts.Values.Sum();

                double ConversionRate(int stageCount) => total == 0 ? 0 : Math.Round(stageCount / (double)total * 100, 1);

                var response = new GetApplicationFunnelAnalyticsResponse(
                    Total: total,
                    New: Count(JobStatus.New),
                    NewConversionRate: ConversionRate(Count(JobStatus.New)),
                    Screening: Count(JobStatus.Screening),
                    ScreeningConversionRate: ConversionRate(Count(JobStatus.Screening)),
                    Shortlisted: Count(JobStatus.Shortlisted),
                    ShortlistedConversionRate: ConversionRate(Count(JobStatus.Shortlisted)),
                    Interview: Count(JobStatus.Interview),
                    InterviewConversionRate: ConversionRate(Count(JobStatus.Interview)),
                    Offered: Count(JobStatus.Offered),
                    OfferedConversionRate: ConversionRate(Count(JobStatus.Offered)),
                    Hired: Count(JobStatus.Hired),
                    HiredConversionRate: ConversionRate(Count(JobStatus.Hired)),
                    Rejected: Count(JobStatus.Rejected),
                    Withdrawn: Count(JobStatus.Withdrawn),
                    InterviewsScheduled: interviewStats.Scheduled,
                    InterviewsCompleted: interviewStats.Completed,
                    InterviewsUpcoming: interviewStats.Upcoming);

                return Result<GetApplicationFunnelAnalyticsResponse>.Success(response, "Funnel analytics retrieved successfully");
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

    public record GetApplicationFunnelAnalyticsResponse(
        int Total,
        int New, double NewConversionRate,
        int Screening, double ScreeningConversionRate,
        int Shortlisted, double ShortlistedConversionRate,
        int Interview, double InterviewConversionRate,
        int Offered, double OfferedConversionRate,
        int Hired, double HiredConversionRate,
        int Rejected,
        int Withdrawn,
        int InterviewsScheduled,
        int InterviewsCompleted,
        int InterviewsUpcoming);
}