using Application.Common.Dtos;
using Application.Common.Repositories;
using MediatR;

namespace Application.Commands
{
    public class UpdateProfessionalProfile
    {
        public record UpdateProfessionalProfileCommand(
            Guid UserId,
            string HeadLine,
            string Summary,
            string? PortfolioUrl,
            string? GitHubUrl,
            string? LinkedInUrl,
            string? ResumeUrl
        ) : IRequest<Result<string>>;

        public class UpdateProfessionalProfileHandler(
            IProfessionalProfileRepository professionalProfileRepository,
            IUnitOfWork unitOfWork)
            : IRequestHandler<
                UpdateProfessionalProfileCommand,
                Result<string>>
        {
            public async Task<Result<string>> Handle(
                UpdateProfessionalProfileCommand request,
                CancellationToken cancellationToken)
            {
                var profile =
                    await professionalProfileRepository
                    .GetByUserIdAsync(request.UserId);


                if (profile is null)
                {
                    return Result<string>.Failure(
                        "Professional profile not found");
                }

                profile.HeadLine = request.HeadLine;
                profile.Summary = request.Summary;
                profile.PortfolioUrl = request.PortfolioUrl;
                profile.GitHubUrl = request.GitHubUrl;
                profile.LinkedInUrl = request.LinkedInUrl;
                profile.ResumeUrl = request.ResumeUrl;
                profile.DateModified = DateTime.UtcNow;


                professionalProfileRepository.Update(profile);

                await unitOfWork.SaveAsync();

                return Result<string>.Success(
                    "Professional profile updated successfully",
                    "updated");
            }
        }
    }
}