using Application.Common.Repositories;
using Application.Common.Dtos;
using MediatR;

namespace Application.Commands.Professional
{
    public class TrackResumeDownload
    {
        public record TrackResumeDownloadCommand(Guid ProfessionalProfileId) : IRequest<Result<string>>;

        public class TrackResumeDownloadHandler(
            IProfessionalProfileRepository professionalProfileRepository,
            IUnitOfWork unitOfWork) : IRequestHandler<TrackResumeDownloadCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(TrackResumeDownloadCommand request, CancellationToken cancellationToken)
            {
                var profile = await professionalProfileRepository.GetByIdAsync(request.ProfessionalProfileId);

                if (profile is null)
                    return Result<string>.Failure("Professional profile not found");

                profile.ResumeDownloadCount += 1;

                professionalProfileRepository.Update(profile);

                await unitOfWork.SaveAsync();

                return Result<string>.Success("Download recorded", "tracked");
            }
        }
    }
}