using Application.Common.Repositories;
using Application.Common.Dtos;
using MediatR;

namespace Application.Commands.Authentication
{
    public class UpdateUserBasicInfo
    {
        public record UpdateUserBasicInfoCommand(
            Guid UserId,
            string FirstName,
            string LastName,
            string? Tel,
            string? Location
            ) : IRequest<Result<string>>;

        public class UpdateUserBasicInfoHandler(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork) : IRequestHandler<UpdateUserBasicInfoCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(UpdateUserBasicInfoCommand request, CancellationToken cancellationToken)
            {
                var user = await userRepository.GetByIdAsync(request.UserId);

                if (user is null)
                    return Result<string>.Failure("User not found");

                user.FirstName = request.FirstName;

                user.LastName = request.LastName;

                user.Tel = request.Tel;

                user.Location = request.Location;

                user.DateModified = DateTime.UtcNow;

                userRepository.UpdateAsync(user);

                await unitOfWork.SaveAsync();

                return Result<string>.Success("Profile updated successfully", "updated");
            }
        }
    }
}