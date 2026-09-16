using Application.Common.Dtos;
using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.Queries.Job
{
    public class GetApplicationsByJob
    {
        public record GetApplicationsByJobQuery(
            Guid JobId,
            PageRequest PageRequest,
            bool UsePaging
        ) : IRequest<Result<PageResponse<GetApplicationsByJobResponse>>>;

        public class GetApplicationsByJobHandler(
            IJobApplicationRepository jobApplicationRepository)
            : IRequestHandler<GetApplicationsByJobQuery, Result<PageResponse<GetApplicationsByJobResponse>>>
        {
            public async Task<Result<PageResponse<GetApplicationsByJobResponse>>> Handle(
                GetApplicationsByJobQuery request,
                CancellationToken cancellationToken)
            {
                var applications = await jobApplicationRepository.GetByJobIdAsync(
                    request.JobId, request.PageRequest, request.UsePaging);

                var response = new PageResponse<GetApplicationsByJobResponse>
                {
                    Items = applications.Items.Select(a => new GetApplicationsByJobResponse(
                        a.Id,
                        a.ProfessionalProfileId,
                        a.ProfessionalProfile.User.FirstName,
                        a.ProfessionalProfile.User.LastName,
                        a.ProfessionalProfile.User.ProfilePictureUrl,
                        a.CoverLetter,
                        a.ResumeUrl,
                        a.JobStatus,
                        a.AppliedAt)).ToList(),
                    TotalCount = applications.TotalCount,
                    PageNumber = applications.PageNumber,
                    PageSize = applications.PageSize
                };

                return Result<PageResponse<GetApplicationsByJobResponse>>.Success(response, "Applications retrieved successfully");
            }
        }
    }

    public record GetApplicationsByJobResponse(
        Guid ApplicationId,
        Guid ProfessionalProfileId,
        string FirstName,
        string LastName,
        string? ProfilePictureUrl,
        string CoverLetter,
        string? ResumeUrl,
        JobStatus JobStatus,
        DateTime AppliedAt);
}