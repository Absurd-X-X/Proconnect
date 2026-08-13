using Application.Common.Dtos;
using Application.Common.Repositories;
using Application.Contract.Settings;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commands.Authentication
{
    public class UploadProfilePicture
    {
        public record UploadProfilePictureCommand(
            Guid UserId,
            IFormFile File
            ) : IRequest<Result<string>>;

        public class UploadProfilePictureHandler(
            IUserRepository userRepository,
            IFileStorage fileStorage,
            IUnitOfWork unitOfWork) : IRequestHandler<UploadProfilePictureCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(UploadProfilePictureCommand request, CancellationToken cancellationToken)
            {
                var user = await userRepository.GetByIdAsync(request.UserId);

                if (user is null)
                    return Result<string>.Failure("User not found");

                if (request.File is null || request.File.Length == 0)
                    return Result<string>.Failure("No file was uploaded");

                if (!string.IsNullOrWhiteSpace(user.ProfilePicturePublicId))
                {
                    await fileStorage.DeleteAsync(user.ProfilePicturePublicId, cancellationToken);
                }

                var uploadResult = await fileStorage.UploadAsync(
                    request.File,
                    "proconnect/profile-pictures",
                    cancellationToken);

                user.ProfilePictureUrl = uploadResult.Url;

                user.ProfilePicturePublicId = uploadResult.PublicId;

                user.DateModified = DateTime.UtcNow;

                await unitOfWork.SaveAsync();

                return Result<string>.Success(uploadResult.Url, "Profile picture updated successfully");
            }
        }
    }
}