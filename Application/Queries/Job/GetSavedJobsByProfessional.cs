using Application.Common.Dtos;
using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.Queries.Job
{
    public class GetSavedJobsByProfessional
    {
        public record GetSavedJobsByProfessionalQuery(
            Guid ProfessionalProfileId,
            PageRequest PageRequest,
            bool UsePaging
        ) : IRequest<Result<PageResponse<GetSavedJobsByProfessionalResponse>>>;

        public class GetSavedJobsByProfessionalHandler(
            ISavedJobRepository savedJobRepository)
            : IRequestHandler<GetSavedJobsByProfessionalQuery, Result<PageResponse<GetSavedJobsByProfessionalResponse>>>
        {
            public async Task<Result<PageResponse<GetSavedJobsByProfessionalResponse>>> Handle(
                GetSavedJobsByProfessionalQuery request,
                CancellationToken cancellationToken)
            {
                var savedJobs = await savedJobRepository.GetByProfessionalProfileIdAsync(
                    request.ProfessionalProfileId, request.PageRequest, request.UsePaging);

                var response = new PageResponse<GetSavedJobsByProfessionalResponse>
                {
                    Items = savedJobs.Items.Select(s => new GetSavedJobsByProfessionalResponse(
                        s.Id,
                        s.JobId,
                        s.Job.Title,
                        s.Job.Company.Name,
                        s.Job.Company.LogoUrl,
                        s.Job.Location,
                        s.Job.WorkPlaceType,
                        s.Job.EmploymentType,
                        s.Job.MinSalary,
                        s.Job.MaxSalary,
                        s.Job.Currency,
                        s.Job.ApplicationDeadline,
                        s.Job.Status,
                        s.SavedAt)).ToList(),
                    TotalCount = savedJobs.TotalCount,
                    PageNumber = savedJobs.PageNumber,
                    PageSize = savedJobs.PageSize
                };

                return Result<PageResponse<GetSavedJobsByProfessionalResponse>>.Success(response, "Saved jobs retrieved successfully");
            }
        }
    }

    public record GetSavedJobsByProfessionalResponse(
        Guid SavedJobId,
        Guid JobId,
        string JobTitle,
        string CompanyName,
        string? CompanyLogoUrl,
        string Location,
        WorkPlaceType WorkPlaceType,
        EmploymentType EmploymentType,
        decimal MinSalary,
        decimal MaxSalary,
        string Currency,
        DateTime ApplicationDeadline,
        JobPostingStatus JobPostingStatus,
        DateTime SavedAt);
}