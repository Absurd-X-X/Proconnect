using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.Queries.Job
{
    public class GetApplicationStatusCounts
    {
        public record GetApplicationStatusCountsQuery(
            Guid RecruiterProfileId,
            Guid? JobId,
            string? Keyword,
            DateTime? AppliedAfter,
            DateTime? AppliedBefore,
            WorkAuthorizationStatus? WorkAuthorization
        ) : IRequest<Result<GetApplicationStatusCountsResponse>>;

        public class GetApplicationStatusCountsHandler(
            IJobApplicationRepository jobApplicationRepository)
            : IRequestHandler<GetApplicationStatusCountsQuery, Result<GetApplicationStatusCountsResponse>>
        {
            public async Task<Result<GetApplicationStatusCountsResponse>> Handle(
                GetApplicationStatusCountsQuery request,
                CancellationToken cancellationToken)
            {
                var filter = new JobApplicationFilter
                {
                    JobId = request.JobId,
                    Keyword = request.Keyword,
                    AppliedAfter = request.AppliedAfter,
                    AppliedBefore = request.AppliedBefore,
                    WorkAuthorization = request.WorkAuthorization
                };

                var counts = await jobApplicationRepository.GetStatusCountsByRecruiterAsync(
                    request.RecruiterProfileId, filter);

                int Count(JobStatus status) => counts.TryGetValue(status, out var c) ? c : 0;

                var response = new GetApplicationStatusCountsResponse(
                    counts.Values.Sum(),
                    Count(JobStatus.New),
                    Count(JobStatus.Screening),
                    Count(JobStatus.Shortlisted),
                    Count(JobStatus.Interview),
                    Count(JobStatus.Offered),
                    Count(JobStatus.Hired),
                    Count(JobStatus.Rejected),
                    Count(JobStatus.Withdrawn));

                return Result<GetApplicationStatusCountsResponse>.Success(response, "Status counts retrieved successfully");
            }
        }
    }

    public record GetApplicationStatusCountsResponse(
        int All,
        int New,
        int Screening,
        int Shortlisted,
        int Interview,
        int Offered,
        int Hired,
        int Rejected,
        int Withdrawn);
}