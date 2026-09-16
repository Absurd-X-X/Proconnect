using Application.Common.Dtos;
using Application.Contract.Settings;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commands.Job
{
    public class UploadApplicationResume
    {
        public record UploadApplicationResumeCommand(
            IFormFile File
        ) : IRequest<Result<UploadApplicationResumeResponse>>;

        public class UploadApplicationResumeHandler(
            IFileStorage fileStorage)
            : IRequestHandler<UploadApplicationResumeCommand, Result<UploadApplicationResumeResponse>>
        {
            public async Task<Result<UploadApplicationResumeResponse>> Handle(
                UploadApplicationResumeCommand request,
                CancellationToken cancellationToken)
            {
                if (request.File is null || request.File.Length == 0)
                    return Result<UploadApplicationResumeResponse>.Failure("No file was uploaded");

                var uploadResult = await fileStorage.UploadAsync(
                    request.File,
                    "proconnect/application-resumes",
                    cancellationToken);

                var response = new UploadApplicationResumeResponse(
                    uploadResult.Url,
                    uploadResult.PublicId,
                    request.File.FileName,
                    request.File.Length);

                return Result<UploadApplicationResumeResponse>.Success(response, "Resume uploaded successfully");
            }
        }
    }

    public record UploadApplicationResumeResponse(
        string ResumeUrl,
        string ResumePublicId,
        string FileName,
        long FileSizeBytes);
}