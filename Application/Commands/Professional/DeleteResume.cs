using Application.Common.Dtos;
using Application.Common.Repositories;
using Application.Contract.Settings;
using MediatR;

namespace Application.Commands.Professional
{
    public class DeleteResume
    {
        public record DeleteResumeCommand(Guid ProfessionalProfileId) : IRequest<Result<string>>;

        public class DeleteResumeHandler(
            IProfessionalProfileRepository professionalProfileRepository,
            IFileStorage fileStorage,
            IUnitOfWork unitOfWork) : IRequestHandler<DeleteResumeCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(DeleteResumeCommand request, CancellationToken cancellationToken)
            {
                var profile = await professionalProfileRepository.GetByIdAsync(request.ProfessionalProfileId);

                if (profile is null)
                    return Result<string>.Failure("Professional profile not found");

                if (string.IsNullOrWhiteSpace(profile.ResumeUrl))
                    return Result<string>.Failure("No resume to delete");

                if (!string.IsNullOrWhiteSpace(profile.ResumePublicId))
                {
                    await fileStorage.DeleteAsync(profile.ResumePublicId, cancellationToken);
                }

                profile.ResumeUrl = null;
                profile.ResumePublicId = null;
                profile.ResumeFileName = null;
                profile.ResumeFileSizeBytes = null;
                profile.ResumeUploadedAt = null;
                profile.DateModified = DateTime.UtcNow;

                professionalProfileRepository.Update(profile);

                await unitOfWork.SaveAsync();

                return Result<string>.Success("Resume deleted successfully", "deleted");
            }
        }
    }
}