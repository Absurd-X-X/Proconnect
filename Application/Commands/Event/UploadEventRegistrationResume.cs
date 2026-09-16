using Application.Common.Dtos;
using Application.Contract.Settings;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commands.Event
{
    public class UploadEventRegistrationResume
    {
        public record UploadEventRegistrationResumeCommand(Guid UserId, IFormFile File) : IRequest<Result<UploadEventRegistrationResumeResponse>>;

        public class UploadEventRegistrationResumeHandler(IFileStorage fileStorage)
            : IRequestHandler<UploadEventRegistrationResumeCommand, Result<UploadEventRegistrationResumeResponse>>
        {
            public async Task<Result<UploadEventRegistrationResumeResponse>> Handle(
                UploadEventRegistrationResumeCommand request,
                CancellationToken cancellationToken)
            {
                var uploadResult = await fileStorage.UploadAsync(request.File, "event-registration-resumes", cancellationToken);

                var response = new UploadEventRegistrationResumeResponse(uploadResult.Url, uploadResult.PublicId);

                return Result<UploadEventRegistrationResumeResponse>.Success(response, "Resume uploaded successfully");
            }
        }
    }

    public record UploadEventRegistrationResumeResponse(string Url, string PublicId);
}