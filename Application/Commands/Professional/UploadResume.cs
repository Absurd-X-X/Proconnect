using Application.Common.Dtos;
using Application.Common.Repositories;
using Application.Contract.Settings;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commands.Professional
{
    public class UploadResume
    {
        public record UploadResumeCommand(
            Guid ProfessionalProfileId,
            IFormFile File
            ) : IRequest<Result<string>>;

        public class UploadResumeHandler(
            IProfessionalProfileRepository professionalProfileRepository,
            IFileStorage fileStorage,
            IUnitOfWork unitOfWork) : IRequestHandler<UploadResumeCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(UploadResumeCommand request, CancellationToken cancellationToken)
            {
                var profile = await professionalProfileRepository.GetByIdAsync(request.ProfessionalProfileId);

                if (profile is null)
                    return Result<string>.Failure("Professional profile not found");

                if (request.File is null || request.File.Length == 0)
                    return Result<string>.Failure("No file was uploaded");

                if (!string.IsNullOrWhiteSpace(profile.ResumePublicId))
                {
                    await fileStorage.DeleteAsync(profile.ResumePublicId, cancellationToken);
                }

                var uploadResult = await fileStorage.UploadAsync(
                    request.File,
                    "proconnect/resumes",
                    cancellationToken);

                profile.ResumeUrl = uploadResult.Url;

                profile.ResumePublicId = uploadResult.PublicId;

                profile.DateModified = DateTime.UtcNow;

                await unitOfWork.SaveAsync();

                return Result<string>.Success(uploadResult.Url, "Resume uploaded successfully");
            }
        }
    }
}