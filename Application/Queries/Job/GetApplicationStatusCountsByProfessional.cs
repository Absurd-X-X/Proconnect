using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.Queries.Job
{
    public class GetApplicationStatusCountsByProfessional
    {
        public record GetApplicationStatusCountsByProfessionalQuery(
            Guid ProfessionalProfileId
        ) : IRequest<Result<GetApplicationStatusCountsByProfessionalResponse>>;

        public class GetApplicationStatusCountsByProfessionalHandler(
            IJobApplicationRepository jobApplicationRepository)
            : IRequestHandler<GetApplicationStatusCountsByProfessionalQuery, Result<GetApplicationStatusCountsByProfessionalResponse>>
        {
            public async Task<Result<GetApplicationStatusCountsByProfessionalResponse>> Handle(
                GetApplicationStatusCountsByProfessionalQuery request,
                CancellationToken cancellationToken)
            {
                var counts = await jobApplicationRepository.GetStatusCountsByProfessionalAsync(request.ProfessionalProfileId);

                int Count(JobStatus status) => counts.TryGetValue(status, out var c) ? c : 0;

                var response = new GetApplicationStatusCountsByProfessionalResponse(
                    counts.Values.Sum(),
                    Count(JobStatus.New),
                    Count(JobStatus.Screening),
                    Count(JobStatus.Shortlisted),
                    Count(JobStatus.Interview),
                    Count(JobStatus.Offered),
                    Count(JobStatus.Hired),
                    Count(JobStatus.Rejected),
                    Count(JobStatus.Withdrawn));

                return Result<GetApplicationStatusCountsByProfessionalResponse>.Success(response, "Status counts retrieved successfully");
            }
        }
    }

    public record GetApplicationStatusCountsByProfessionalResponse(
        int All, int New, int Screening, int Shortlisted, int Interview, int Offered, int Hired, int Rejected, int Withdrawn);
}