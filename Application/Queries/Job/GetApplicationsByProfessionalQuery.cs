using Application.Common.Dtos;
using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.Queries.Job
{
    public class GetApplicationsByProfessional
    {
        public record GetApplicationsByProfessionalQuery(
            Guid ProfessionalProfileId,
            PageRequest PageRequest,
            bool UsePaging
        ) : IRequest<Result<PageResponse<GetApplicationsByProfessionalResponse>>>;

        public class GetApplicationsByProfessionalHandler(
            IJobApplicationRepository jobApplicationRepository)
            : IRequestHandler<GetApplicationsByProfessionalQuery, Result<PageResponse<GetApplicationsByProfessionalResponse>>>
        {
            public async Task<Result<PageResponse<GetApplicationsByProfessionalResponse>>> Handle(
                GetApplicationsByProfessionalQuery request,
                CancellationToken cancellationToken)
            {
                var applications = await jobApplicationRepository.GetByProfessionalProfileIdAsync(
                    request.ProfessionalProfileId, request.PageRequest, request.UsePaging);

                var response = new PageResponse<GetApplicationsByProfessionalResponse>
                {
                    Items = applications.Items.Select(a => new GetApplicationsByProfessionalResponse(
                        a.Id,
                        a.JobId,
                        a.Job.Title,
                        a.Job.Company.Name,
                        a.Job.Company.LogoUrl,
                        a.JobStatus,
                        a.AppliedAt)).ToList(),
                    TotalCount = applications.TotalCount,
                    PageNumber = applications.PageNumber,
                    PageSize = applications.PageSize
                };

                return Result<PageResponse<GetApplicationsByProfessionalResponse>>.Success(response, "Applications retrieved successfully");
            }
        }
    }

    public record GetApplicationsByProfessionalResponse(
        Guid ApplicationId,
        Guid JobId,
        string JobTitle,
        string CompanyName,
        string? CompanyLogoUrl,
        JobStatus JobStatus,
        DateTime AppliedAt);
}