using Application.Common.Dtos;
using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Queries.Analytics
{
    public class GetJobAnalytics
    {
        public record GetJobAnalyticsQuery(
            Guid CallerRecruiterProfileId,
            DateRangePreset Preset,
            DateTime? CustomStart,
            DateTime? CustomEnd,
            Guid? JobId,
            PageRequest PageRequest,
            bool UsePaging
        ) : IRequest<Result<PageResponse<JobAnalyticsResponse>>>;

        public class GetJobAnalyticsHandler(
            IRecruiterProfileRepository recruiterProfileRepository,
            IJobRepository jobRepository,
            IJobApplicationRepository jobApplicationRepository)
            : IRequestHandler<GetJobAnalyticsQuery, Result<PageResponse<JobAnalyticsResponse>>>
        {
            public async Task<Result<PageResponse<JobAnalyticsResponse>>> Handle(
                GetJobAnalyticsQuery request,
                CancellationToken cancellationToken)
            {
                var callingRecruiter = await recruiterProfileRepository.GetByIdAsync(request.CallerRecruiterProfileId);

                if (callingRecruiter is null || callingRecruiter.Status != RecruiterStatus.Active || callingRecruiter.CompanyId is null)
                    return Result<PageResponse<JobAnalyticsResponse>>.Failure("You are not authorized to view this data");

                var companyId = callingRecruiter.CompanyId.Value;
                var (start, end) = ResolveRange(request.Preset, request.CustomStart, request.CustomEnd);

                var jobsPage = await jobRepository.SearchAsync(
                    new JobSearchFilter { CompanyId = companyId },
                    request.PageRequest,
                    request.UsePaging);

                var jobs = request.JobId.HasValue
                    ? jobsPage.Items.Where(j => j.Id == request.JobId.Value).ToList()
                    : jobsPage.Items.ToList();

                // Single grouped query for every job's status breakdown, instead
                // of one query per job — was the flagged N+1 shape.
                var statusCountsByJob = await jobApplicationRepository.GetStatusCountsGroupedByJobAsync(companyId, start, end);

                var items = jobs.Select(job =>
                {
                    var statusCounts = statusCountsByJob.TryGetValue(job.Id, out var counts)
                        ? counts
                        : new Dictionary<JobStatus, int>();

                    return new JobAnalyticsResponse(
                        job.Id,
                        job.Title,
                        job.Status,
                        job.DateCreated,
                        statusCounts.Values.Sum(),
                        statusCounts.TryGetValue(JobStatus.Hired, out var hired) ? hired : 0,
                        statusCounts.TryGetValue(JobStatus.Rejected, out var rejected) ? rejected : 0,
                        (DateTime.UtcNow - job.DateCreated).Days);
                }).ToList();

                var response = new PageResponse<JobAnalyticsResponse>
                {
                    Items = items,
                    TotalCount = request.JobId.HasValue ? items.Count : jobsPage.TotalCount,
                    PageNumber = jobsPage.PageNumber,
                    PageSize = jobsPage.PageSize
                };

                return Result<PageResponse<JobAnalyticsResponse>>.Success(response, "Job analytics retrieved successfully");
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

    public record JobAnalyticsResponse(
        Guid JobId,
        string Title,
        JobPostingStatus Status,
        DateTime DateCreated,
        int TotalApplications,
        int HiredCount,
        int RejectedCount,
        int DaysOpen);
}