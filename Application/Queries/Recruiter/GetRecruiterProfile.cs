using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Queries.Recruiter
{
    public class GetRecruiterProfile
    {
        public record RecruiterProfileResponse(
            Guid Id,
            Guid UserId,
            string FullName,
            string? ProfilePictureUrl,
            string Email,
            string? Tel,
            string? Location,
            string Bio,
            string? JobTitle,
            string? Department,
            bool IsCompanyAdmin,
            RecruiterStatus Status,
            DateTime DateCreated,
            Guid? CompanyId,
            string? CompanyName,
            string? CompanyLogoUrl,
            string? CompanyIndustry,
            string? CompanyCompanySize,
            int OpenJobCount,
            int TotalHireCount
        );

        public record GetRecruiterProfileQuery(Guid RequestingUserId) : IRequest<Result<RecruiterProfileResponse>>;

        public class GetRecruiterProfileHandler(
            IRecruiterProfileRepository recruiterProfileRepository)
            : IRequestHandler<GetRecruiterProfileQuery, Result<RecruiterProfileResponse>>
        {
            public async Task<Result<RecruiterProfileResponse>> Handle(
                GetRecruiterProfileQuery request,
                CancellationToken cancellationToken)
            {
                var profile = await recruiterProfileRepository.GetByUserIdAsync(request.RequestingUserId);

                if (profile is null)
                {
                    return Result<RecruiterProfileResponse>.Failure("Recruiter profile not found");
                }

                var openJobs = profile.Jobs.Count(j => j.IsActive);

                var totalHires = profile.Jobs
                    .SelectMany(j => j.JobApplications)
                    .Count(a => a.JobStatus == JobStatus.Hired);

                var result = new RecruiterProfileResponse(
                    profile.Id,
                    profile.UserId,
                    $"{profile.User.FirstName} {profile.User.LastName}",
                    profile.User.ProfilePictureUrl,
                    profile.User.Email,
                    profile.User.Tel,
                    profile.User.Location,
                    profile.User.Bio,
                    profile.JobTitle,
                    profile.Department,
                    profile.IsCompanyAdmin,
                    profile.Status,
                    profile.DateCreated,
                    profile.CompanyId,
                    profile.Company?.Name,
                    profile.Company?.LogoUrl,
                    profile.Company?.Industry,
                    profile.Company?.CompanySize,
                    openJobs,
                    totalHires);

                return Result<RecruiterProfileResponse>.Success(result, "Recruiter profile retrieved");
            }
        }
    }
}