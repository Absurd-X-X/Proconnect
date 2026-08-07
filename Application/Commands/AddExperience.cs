using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Commands
{
    public class AddExperience
    {
        public record AddExperienceCommand(
            Guid ProfessionalProfileId,
            string CompanyName,
            string Position,
            string Description,
            DateTime StartDate,
            DateTime EndDate,
            bool IsCurrent
        ) : IRequest<Result<string>>;

        public class AddExperienceHandler(
            IExperienceRepository experienceRepository,
            IUnitOfWork unitOfWork)
            : IRequestHandler<
                AddExperience.AddExperienceCommand,
                Result<string>>
        {
            public async Task<Result<string>> Handle(
                AddExperience.AddExperienceCommand request,
                CancellationToken cancellationToken)
            {
                var experience = new Experience
                {
                    ProfessionalProfileId = request.ProfessionalProfileId,

                    CompanyName = request.CompanyName,

                    JobTitle = request.Position,

                    Description = request.Description,

                    StartDate = request.StartDate,

                    EndDate = request.EndDate,

                    IsCurrentJob = request.IsCurrent
                };


                await experienceRepository.AddAsync(experience);

                await unitOfWork.SaveAsync();

                return Result<string>.Success(
                    "Experience added successfully",
                    "created");
            }
        }
    }
}