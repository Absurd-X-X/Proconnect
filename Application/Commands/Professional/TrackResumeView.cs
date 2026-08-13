using Application.Common.Repositories;
using Application.Common.Dtos;
using MediatR;

namespace Application.Commands.Professional
{
    public class TrackResumeView
    {
        public record TrackResumeViewCommand(Guid ProfessionalProfileId) : IRequest<Result<string>>;

        public class TrackResumeViewHandler(
            IProfessionalProfileRepository professionalProfileRepository,
            IUnitOfWork unitOfWork) : IRequestHandler<TrackResumeViewCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(TrackResumeViewCommand request, CancellationToken cancellationToken)
            {
                var profile = await professionalProfileRepository.GetByIdAsync(request.ProfessionalProfileId);

                if (profile is null)
                    return Result<string>.Failure("Professional profile not found");

                profile.ResumeViewCount += 1;

                professionalProfileRepository.Update(profile);

                await unitOfWork.SaveAsync();

                return Result<string>.Success("View recorded", "tracked");
            }
        }
    }
}