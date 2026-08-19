using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.Queries.Professional
{
    public class GetProfessionalProfile
    {
        public record GetProfessionalProfileQuery(Guid Id) : IRequest<Result<GetProfessionalProfileResponse>>;

        public class GetProfessionalProfileHandler(
            IProfessionalProfileRepository professionalProfileRepository) : IRequestHandler<GetProfessionalProfileQuery, Result<GetProfessionalProfileResponse>>
        {
            public async Task<Result<GetProfessionalProfileResponse>> Handle(GetProfessionalProfileQuery request, CancellationToken cancellationToken)
            {
                var profile = await professionalProfileRepository.GetByIdAsync(request.Id);

                if (profile is null)
                    return Result<GetProfessionalProfileResponse>.Failure("Professional profile not found");

                var response = new GetProfessionalProfileResponse(
                    profile.Id,
                    profile.UserId,
                    profile.User.FirstName,
                    profile.User.LastName,
                    profile.User.Email,
                    profile.User.Tel,
                    profile.User.Location,
                    profile.User.ProfilePictureUrl,
                    profile.User.IsVerified,
                    profile.HeadLine,
                    profile.Summary,
                    profile.GitHubUrl,
                    profile.LinkedInUrl,
                    profile.WebsiteUrl,
                    profile.ResumeUrl,
                    profile.ResumeFileName,
                    profile.ResumeFileSizeBytes,
                    profile.ResumeUploadedAt,
                    profile.ResumeViewCount,
                    profile.ResumeDownloadCount,
                    profile.UserStatus,
                    profile.AvailabilityStatus,
                    profile.PreferredJobTypes,
                    profile.PreferredLocations,
                    profile.EarliestStartDate,
                    profile.WillingToRelocate,
                    profile.WorkAuthorization,
                    profile.AvailabilityVisibility,
                    profile.PortfolioLinks
                        .OrderByDescending(l => l.DateCreated)
                        .Select(l => new GetPortfolioLinksByProfileResponse(
                            l.Id, l.Title, l.Url, l.LinkType, l.Description, l.ThumbnailUrl, l.ViewCount, l.ClickCount))
                        .ToList(),
                    profile.Educations
                        .OrderByDescending(e => e.StartDate)
                        .Select(e => new GetEducationsByProfileResponse(
                            e.Id, e.Institution, e.Degree, e.FieldOfStudy, e.StartDate, e.EndDate, e.Grade))
                        .ToList(),
                    profile.Experiences
                        .OrderByDescending(e => e.IsCurrentJob)
                        .ThenByDescending(e => e.StartDate)
                        .Select(e => new GetExperiencesByProfileResponse(
                            e.Id, e.CompanyName, e.JobTitle, e.EmploymentType, e.Location, e.StartDate, e.EndDate, e.IsCurrentJob))
                        .ToList(),
                    profile.Certificates
                        .OrderByDescending(c => c.IssueDate)
                        .Select(c => new GetCertificatesByProfileResponse(
                            c.Id, c.Name, c.IssuingOrganization, c.IssueDate, c.ExpireDate))
                        .ToList(),
                    profile.Projects
                        .OrderByDescending(p => p.DateCreated)
                        .Select(p => new GetProjectsByProfileResponse(p.Id, p.Title, p.ProjectUrl))
                        .ToList(),
                    profile.ProfessionalSkills
                        .Select(s => new GetProfessionalSkillsByProfileRespone(
                            s.Id, s.SkillId, s.Skill.Name, s.Level, s.YearsOfExperience))
                        .ToList());

                return Result<GetProfessionalProfileResponse>.Success(response, "Profile retrieved successfully");
            }
        }
    }

    public record GetProfessionalProfileResponse(
        Guid Id,
        Guid UserId,
        string FirstName,
        string LastName,
        string? Email,
        string? Phone,
        string? Location,
        string? ProfilePicture,
        bool IsVerified,
        string? HeadLine,
        string? Summary,
        string? GitHubUrl,
        string? LinkedInUrl,
        string? WebsiteUrl,
        string? ResumeUrl,
        string? ResumeFileName,
        long? ResumeFileSizeBytes,
        DateTime? ResumeUploadedAt,
        int ResumeViewCount,
        int ResumeDownloadCount,
        UserStatus UserStatus,
        AvailabilityStatus AvailabilityStatus,
        List<EmploymentType> PreferredJobTypes,
        List<string> PreferredLocations,
        DateTime? EarliestStartDate,
        bool WillingToRelocate,
        WorkAuthorizationStatus WorkAuthorization,
        AvailabilityVisibility AvailabilityVisibility,
        List<GetPortfolioLinksByProfileResponse> PortfolioLinks,
        List<GetEducationsByProfileResponse> Educations,
        List<GetExperiencesByProfileResponse> Experiences,
        List<GetCertificatesByProfileResponse> Certificates,
        List<GetProjectsByProfileResponse> Projects,
        List<GetProfessionalSkillsByProfileRespone> Skills);

    public record GetPortfolioLinksByProfileResponse(
    Guid Id,
    string Title,
    string Url,
    PortfolioLinkType LinkType,
    string? Description,
    string? ThumbnailUrl,
    int ViewCount,
    int ClickCount);
}