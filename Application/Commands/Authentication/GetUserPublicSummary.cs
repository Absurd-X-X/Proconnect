using Application.Common.Dtos;
using Application.Common.Pagenation;
using Application.Common.Repositories;
using MediatR;

namespace Application.Queries.Users
{
    public class GetUserPublicSummary
    {
        public record GetUserPublicSummaryQuery(Guid UserId, Guid CurrentUserId) : IRequest<Result<UserPublicSummaryResponse>>;

        public class GetUserPublicSummaryHandler(
            IUserRepository userRepository,
            IProfessionalProfileRepository professionalProfileRepository,
            IRecruiterProfileRepository recruiterProfileRepository,
            IUserFollowRepository followRepository)
            : IRequestHandler<GetUserPublicSummaryQuery, Result<UserPublicSummaryResponse>>
        {
            public async Task<Result<UserPublicSummaryResponse>> Handle(GetUserPublicSummaryQuery request, CancellationToken cancellationToken)
            {
                var user = await userRepository.GetByIdAsync(request.UserId);

                if (user is null)
                {
                    return Result<UserPublicSummaryResponse>.Failure("User not found");
                }

                string? headline = null;
                string? location = user.Location;

                if (user.Role.Equals("Recruiter", StringComparison.OrdinalIgnoreCase))
                {
                    var recruiterProfile = await recruiterProfileRepository.GetByUserIdAsync(user.Id);

                    if (recruiterProfile is not null)
                    {
                        headline = recruiterProfile.Company is not null
                            ? $"{recruiterProfile.JobTitle} at {recruiterProfile.Company.Name}"
                            : recruiterProfile.JobTitle;
                    }
                }
                else
                {
                    var professionalProfile = await professionalProfileRepository.GetByUserIdAsync(user.Id);
                    headline = professionalProfile?.HeadLine;
                }

                var followerCount = await followRepository.GetFollowerCountAsync(user.Id);

                var isFollowedByMe = request.CurrentUserId != user.Id
                    && await followRepository.GetByFollowerAndFollowingAsync(request.CurrentUserId, user.Id) is not null;

                var response = new UserPublicSummaryResponse(
                    user.Id,
                    user.FirstName,
                    user.LastName,
                    user.ProfilePictureUrl,
                    headline,
                    location,
                    user.IsVerified,
                    followerCount,
                    isFollowedByMe);

                return Result<UserPublicSummaryResponse>.Success(response, "Profile summary retrieved successfully");
            }
        }
    }

    public record UserPublicSummaryResponse(
        Guid UserId,
        string FirstName,
        string LastName,
        string? ProfilePictureUrl,
        string? Headline,
        string? Location,
        bool IsVerified,
        int FollowerCount,
        bool IsFollowedByMe);
}