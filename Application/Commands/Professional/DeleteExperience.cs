using Application.Common.Repositories;
using Application.Common.Dtos;
using MediatR;

namespace Application.Commands.Professional
{
    public class DeleteExperience
    {
        public record DeleteExperienceCommand(Guid Id, bool DeletePermanently) : IRequest<Result<string>>;

        public class DeleteExperienceHandler(
            IExperienceRepository experienceRepository,
            IUnitOfWork unitOfWork) : IRequestHandler<DeleteExperienceCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(DeleteExperienceCommand request, CancellationToken cancellationToken)
            {
                var experience = await experienceRepository.GetByIdAsync(request.Id);

                if (experience is null)
                    return Result<string>.Failure("Experience record not found");

                if (request.DeletePermanently)
                {
                    experienceRepository.Delete(experience);
                }

                else
                {
                    experience.IsDeleted  = true;
                    experienceRepository.UpdateAsync(experience);
                }


                await unitOfWork.SaveAsync();

                return Result<string>.Success("Experience deleted successfully", "deleted");
            }
        }
    }
}