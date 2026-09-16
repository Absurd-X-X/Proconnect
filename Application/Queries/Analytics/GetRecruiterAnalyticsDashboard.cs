using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Queries.Analytics
{
    public class GetRecruiterAnalyticsDashboard
    {
        public record GetRecruiterAnalyticsDashboardQuery(
            Guid CallerRecruiterProfileId,
            DateRangePreset Preset,
            DateTime? CustomStart,
            DateTime? CustomEnd
        ) : IRequest<Result<GetRecruiterAnalyticsDashboardResponse>>;

        public class GetRecruiterAnalyticsDashboardHandler(
            IRecruiterProfileRepository recruiterProfileRepository,
            IJobRepository jobRepository,
            IJobApplicationRepository jobApplicationRepository)
            : IRequestHandler<GetRecruiterAnalyticsDashboardQuery, Result<GetRecruiterAnalyticsDashboardResponse>>
        {
            public async Task<Result<GetRecruiterAnalyticsDashboardResponse>> Handle(
                GetRecruiterAnalyticsDashboardQuery request,
                CancellationToken cancellationToken)
            {
                var callingRecruiter = await recruiterProfileRepository.GetByIdAsync(request.CallerRecruiterProfileId);

                if (callingRecruiter is null || callingRecruiter.Status != RecruiterStatus.Active || callingRecruiter.CompanyId is null)
                    return Result<GetRecruiterAnalyticsDashboardResponse>.Failure("You are not authorized to view this dashboard");

                var companyId = callingRecruiter.CompanyId.Value;
                var (start, end) = ResolveRange(request.Preset, request.CustomStart, request.CustomEnd);
                var span = end - start;
                var prevStart = start - span;
                var prevEnd = start;

                var jobStatusCounts = await jobRepository.GetStatusCountsByCompanyAsync(companyId);

                var rangeFilter = new JobApplicationFilter { AppliedAfter = start, AppliedBefore = end };
                var applicationStatusCounts = await jobApplicationRepository.GetStatusCountsByCompanyAsync(companyId, rangeFilter);

                var prevRangeFilter = new JobApplicationFilter { AppliedAfter = prevStart, AppliedBefore = prevEnd };
                var prevApplicationStatusCounts = await jobApplicationRepository.GetStatusCountsByCompanyAsync(companyId, prevRangeFilter);

                var applicationsTrend = await jobApplicationRepository.GetApplicationsTrendByCompanyAsync(companyId, start, end);
                var topJobTitles = await jobRepository.GetTopJobTitlesByApplicationsAsync(companyId, start, end, 5);
                var recruiterPerformance = await jobApplicationRepository.GetRecruiterPerformanceByCompanyAsync(companyId, start, end);
                var interviewStats = await jobApplicationRepository.GetInterviewStatsByCompanyAsync(companyId, start, end);

                var myApplicationFilter = new JobApplicationFilter { AppliedAfter = start, AppliedBefore = end };
                var myStatusCounts = await jobApplicationRepository.GetStatusCountsByRecruiterAsync(request.CallerRecruiterProfileId, myApplicationFilter);

                var totalApplications = applicationStatusCounts.Values.Sum();
                var prevTotalApplications = prevApplicationStatusCounts.Values.Sum();

                var response = new GetRecruiterAnalyticsDashboardResponse(
                    ActiveJobs: jobStatusCounts.TryGetValue(JobPostingStatus.Active, out var active) ? active : 0,
                    DraftJobs: jobStatusCounts.TryGetValue(JobPostingStatus.Draft, out var draft) ? draft : 0,
                    ClosedJobs: jobStatusCounts.TryGetValue(JobPostingStatus.Closed, out var closed) ? closed : 0,

                    TotalApplicationsInRange: totalApplications,
                    ApplicationsGrowthPercent: PercentChange(totalApplications, prevTotalApplications),
                    ApplicationsTrend: applicationsTrend.Select(x => new TrendPointResponse(x.Date, x.Count)).ToList(),

                    Funnel: new FunnelResponse(
                        Applied: applicationStatusCounts.Values.Sum(),
                        Screening: applicationStatusCounts.TryGetValue(JobStatus.Screening, out var scr) ? scr : 0,
                        Shortlisted: applicationStatusCounts.TryGetValue(JobStatus.Shortlisted, out var sl) ? sl : 0,
                        Interview: applicationStatusCounts.TryGetValue(JobStatus.Interview, out var iv) ? iv : 0,
                        Offered: applicationStatusCounts.TryGetValue(JobStatus.Offered, out var off) ? off : 0,
                        Hired: applicationStatusCounts.TryGetValue(JobStatus.Hired, out var hir) ? hir : 0,
                        Rejected: applicationStatusCounts.TryGetValue(JobStatus.Rejected, out var rej) ? rej : 0,
                        Withdrawn: applicationStatusCounts.TryGetValue(JobStatus.Withdrawn, out var wd) ? wd : 0),

                    TopJobRoles: topJobTitles.Select(t => new TopJobRoleResponse(t.Title, t.Count)).ToList(),

                    RecruiterPerformance: recruiterPerformance.Select(r => new RecruiterPerformanceResponse(
                        r.RecruiterProfileId, r.FirstName, r.LastName, r.HiredCount, r.AvgDaysToHire)).ToList(),

                    InterviewsScheduled: interviewStats.Scheduled,
                    InterviewsCompleted: interviewStats.Completed,
                    InterviewsUpcoming: interviewStats.Upcoming,

                    MyJobsApplicationsCount: myStatusCounts.Values.Sum(),
                    MyJobsHiredCount: myStatusCounts.TryGetValue(JobStatus.Hired, out var myHired) ? myHired : 0);

                return Result<GetRecruiterAnalyticsDashboardResponse>.Success(response, "Recruiter dashboard analytics retrieved successfully");
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

    public record GetRecruiterAnalyticsDashboardResponse(
        int ActiveJobs,
        int DraftJobs,
        int ClosedJobs,

        int TotalApplicationsInRange,
        double ApplicationsGrowthPercent,
        List<TrendPointResponse> ApplicationsTrend,

        FunnelResponse Funnel,

        List<TopJobRoleResponse> TopJobRoles,

        List<RecruiterPerformanceResponse> RecruiterPerformance,

        int InterviewsScheduled,
        int InterviewsCompleted,
        int InterviewsUpcoming,

        int MyJobsApplicationsCount,
        int MyJobsHiredCount);


    public record FunnelResponse(
        int Applied, int Screening, int Shortlisted, int Interview,
        int Offered, int Hired, int Rejected, int Withdrawn);

    public record TopJobRoleResponse(string Title, int ApplicationCount);

    public record RecruiterPerformanceResponse(
        Guid RecruiterProfileId, string FirstName, string LastName, int HiredCount, double AvgDaysToHire);
}