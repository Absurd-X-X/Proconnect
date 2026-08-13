using Application.Common.Repositories;
using Application.Common.Dtos;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Commands.Professional
{
    public class AddExperience
    {
        public record AddExperienceCommand(
            Guid ProfessionalProfileId,
            string CompanyName,
            string JobTitle,
            EmploymentType EmploymentType,
            string Location,
            DateTime StartDate,
            DateTime EndDate,
            bool IsCurrentJob,
            string Description,
            string CreatedBy
            ) : IRequest<Result<AddExperienceResponse>>;

        public class AddExperienceHandler(
            IProfessionalProfileRepository professionalProfileRepository,
            IExperienceRepository experienceRepository,
            IUnitOfWork unitOfWork) : IRequestHandler<AddExperienceCommand, Result<AddExperienceResponse>>
        {
            public async Task<Result<AddExperienceResponse>> Handle(AddExperienceCommand request, CancellationToken cancellationToken)
            {
                var profile = await professionalProfileRepository.GetByIdAsync(request.ProfessionalProfileId);

                if (profile is null)
                    return Result<AddExperienceResponse>.Failure("Professional profile not found");

                var experience = new Experience
                {
                    ProfessionalProfileId = profile.Id,
                    CompanyName = request.CompanyName,
                    JobTitle = request.JobTitle,
                    EmploymentType = request.EmploymentType,
                    Location = request.Location,
                    StartDate = request.StartDate,
                    EndDate = request.IsCurrentJob ? request.StartDate : request.EndDate,
                    IsCurrentJob = request.IsCurrentJob,
                    Description = request.Description,
                    CreatedBy = request.CreatedBy
                };

                await experienceRepository.CreateAsync(experience);

                await unitOfWork.SaveAsync();

                return Result<AddExperienceResponse>.Success(
                    new AddExperienceResponse(experience.Id, experience.CompanyName, experience.JobTitle),
                    "Experience added successfully");
            }
        }
    }

    public record AddExperienceResponse(Guid Id, string CompanyName, string JobTitle);
}