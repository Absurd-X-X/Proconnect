using Application.Common.Repositories;
using Application.Common.Dtos;
using Domain.Enums;
using MediatR;

namespace Application.Queries
{
    public class GetExperienceById
    {
        public record GetExperienceByIdQuery(Guid Id) : IRequest<Result<GetExperienceByIdResponse>>;

        public class GetExperienceByIdHandler(
            IExperienceRepository experienceRepository) : IRequestHandler<GetExperienceByIdQuery, Result<GetExperienceByIdResponse>>
        {
            public async Task<Result<GetExperienceByIdResponse>> Handle(GetExperienceByIdQuery request, CancellationToken cancellationToken)
            {
                var experience = await experienceRepository.GetByIdAsync(request.Id);

                if (experience is null)
                    return Result<GetExperienceByIdResponse>.Failure("Experience record not found");

                var response = new GetExperienceByIdResponse(
                    experience.Id,
                    experience.ProfessionalProfileId,
                    experience.CompanyName,
                    experience.JobTitle,
                    experience.EmploymentType,
                    experience.Location,
                    experience.StartDate,
                    experience.EndDate,
                    experience.IsCurrentJob,
                    experience.Description);

                return Result<GetExperienceByIdResponse>.Success(response, "Experience retrieved successfully");
            }
        }
    }

    public record GetExperienceByIdResponse(
        Guid Id,
        Guid ProfessionalProfileId,
        string CompanyName,
        string JobTitle,
        EmploymentType EmploymentType,
        string Location,
        DateTime StartDate,
        DateTime EndDate,
        bool IsCurrentJob,
        string Description);
}