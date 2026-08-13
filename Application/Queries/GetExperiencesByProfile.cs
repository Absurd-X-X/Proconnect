using Application.Common.Pagenation;
using Application.Common.Repositories;
using Application.Common.Dtos;
using Domain.Enums;
using MediatR;

namespace Application.Queries
{
    public class GetExperiencesByProfile
    {
        public record GetExperiencesByProfileQuery(
            Guid ProfessionalProfileId,
            PageRequest PageRequest,
            bool UsePaging
            ) : IRequest<Result<PageResponse<GetExperiencesByProfileResponse>>>;

        public class GetExperiencesByProfileHandler(
            IExperienceRepository experienceRepository) : IRequestHandler<GetExperiencesByProfileQuery, Result<PageResponse<GetExperiencesByProfileResponse>>>
        {
            public async Task<Result<PageResponse<GetExperiencesByProfileResponse>>> Handle(GetExperiencesByProfileQuery request, CancellationToken cancellationToken)
            {
                var experiences = await experienceRepository.GetByProfessionalProfileIdAsync(
                    request.PageRequest,
                    request.UsePaging,
                    request.ProfessionalProfileId);

                var items = experiences.Items.Select(e => new GetExperiencesByProfileResponse(
                    e.Id,
                    e.CompanyName,
                    e.JobTitle,
                    e.EmploymentType,
                    e.Location,
                    e.StartDate,
                    e.EndDate,
                    e.IsCurrentJob)).ToList();

                var response = new PageResponse<GetExperiencesByProfileResponse>
                {
                    Items = items,
                    TotalCount = experiences.TotalCount,
                    PageNumber = experiences.PageNumber,
                    PageSize = experiences.PageSize
                };

                return Result<PageResponse<GetExperiencesByProfileResponse>>.Success(response, "Experiences retrieved successfully");
            }
        }
    }

    public record GetExperiencesByProfileResponse(
        Guid Id,
        string CompanyName,
        string JobTitle,
        EmploymentType EmploymentType,
        string Location,
        DateTime StartDate,
        DateTime EndDate,
        bool IsCurrentJob);
}