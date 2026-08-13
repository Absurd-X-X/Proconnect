using Application.Common.Repositories;
using Application.Common.Dtos;
using Domain.Enums;
using MediatR;

namespace Application.Commands.Professional
{
    public class UpdateExperience
    {
        public record UpdateExperienceCommand(
            Guid Id,
            string CompanyName,
            string JobTitle,
            EmploymentType EmploymentType,
            string Location,
            DateTime StartDate,
            DateTime EndDate,
            bool IsCurrentJob,
            string Description
            ) : IRequest<Result<string>>;

        public class UpdateExperienceHandler(
            IExperienceRepository experienceRepository,
            IUnitOfWork unitOfWork) : IRequestHandler<UpdateExperienceCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(UpdateExperienceCommand request, CancellationToken cancellationToken)
            {
                var experience = await experienceRepository.GetByIdAsync(request.Id);

                if (experience is null)
                    return Result<string>.Failure("Experience record not found");

                experience.CompanyName = request.CompanyName;

                experience.JobTitle = request.JobTitle;

                experience.EmploymentType = request.EmploymentType;

                experience.Location = request.Location;

                experience.StartDate = request.StartDate;

                experience.IsCurrentJob = request.IsCurrentJob;

                experience.EndDate = request.IsCurrentJob ? request.StartDate : request.EndDate;

                experience.Description = request.Description;

                experienceRepository.UpdateAsync(experience);

                await unitOfWork.SaveAsync();

                return Result<string>.Success("Experience updated successfully", "updated");
            }
        }
    }
}