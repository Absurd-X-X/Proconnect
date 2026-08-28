using Application.Common.Dtos;
using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Queries.Connections
{
    public class GetConnectionSuggestions
    {
        public record GetConnectionSuggestionsQuery(Guid UserId, SuggestionFilter Filter, int MaxResults = 10)
            : IRequest<Result<List<SuggestionResponse>>>;

        public class GetConnectionSuggestionsHandler(
            IUserRepository userRepository,
            IUserConnectionRepository connectionRepository,
            IProfessionalProfileRepository professionalProfileRepository,
            IRecruiterProfileRepository recruiterProfileRepository,
            IExperienceRepository experienceRepository,
            IEducationRepository educationRepository)
            : IRequestHandler<GetConnectionSuggestionsQuery, Result<List<SuggestionResponse>>>
        {
            private const int CandidatePoolSize = 50;

            public async Task<Result<List<SuggestionResponse>>> Handle(GetConnectionSuggestionsQuery request, CancellationToken cancellationToken)
            {
                var currentUser = await userRepository.GetByIdAsync(request.UserId);

                if (currentUser is null)
                {
                    return Result<List<SuggestionResponse>>.Failure("User not found");
                }

                var myProfile = await BuildMatchProfileAsync(currentUser);

                var relatedIds = await connectionRepository.GetRelatedUserIdsAsync(request.UserId);

                var pool = await userRepository.GetAllAsync(
                    new PageRequest { PageNumber = 1, PageSize = CandidatePoolSize }, true);

                var candidates = pool.Items
                    .Where(u => u.Id != request.UserId && !relatedIds.Contains(u.Id))
                    .ToList();

                var scored = new List<SuggestionResponse>();

                foreach (var candidate in candidates)
                {
                    var candidateProfile = await BuildMatchProfileAsync(candidate);

                    var mutualCount = await connectionRepository.GetMutualConnectionsCountAsync(request.UserId, candidate.Id);

                    var isSameCompany = myProfile.CompanyId is not null && myProfile.CompanyId == candidateProfile.CompanyId;
                    var isSameIndustry = !isSameCompany
                        && myProfile.Industry is not null
                        && string.Equals(myProfile.Industry, candidateProfile.Industry, StringComparison.OrdinalIgnoreCase);
                    var isAlumni = myProfile.Institutions.Intersect(candidateProfile.Institutions, StringComparer.OrdinalIgnoreCase).Any();

                    if (request.Filter == SuggestionFilter.SameCompany && !isSameCompany) continue;
                    if (request.Filter == SuggestionFilter.SameIndustry && !isSameIndustry) continue;
                    if (request.Filter == SuggestionFilter.Alumni && !isAlumni) continue;

                    var score = (mutualCount * 2) + (isSameCompany ? 10 : 0) + (isSameIndustry ? 5 : 0) + (isAlumni ? 8 : 0);

                    scored.Add(new SuggestionResponse(
                        candidate.Id,
                        candidate.FirstName,
                        candidate.LastName,
                        candidate.ProfilePictureUrl,
                        candidateProfile.Headline,
                        candidateProfile.CompanyName,
                        candidateProfile.Location,
                        mutualCount,
                        isSameCompany,
                        isSameIndustry,
                        isAlumni,
                        score));
                }

                var results = scored
                    .OrderByDescending(s => s.Score)
                    .ThenByDescending(s => s.MutualConnectionsCount)
                    .Take(request.MaxResults)
                    .ToList();

                return Result<List<SuggestionResponse>>.Success(results, "Suggestions retrieved successfully");
            }

            private async Task<MatchProfile> BuildMatchProfileAsync(User user)
            {
                if (user.Role.Equals("Recruiter", StringComparison.OrdinalIgnoreCase))
                {
                    var recruiterProfile = await recruiterProfileRepository.GetByUserIdAsync(user.Id);

                    if (recruiterProfile?.Company is not null)
                    {
                        return new MatchProfile(
                            recruiterProfile.JobTitle,
                            recruiterProfile.Company.Name,
                            recruiterProfile.Company.Id,
                            recruiterProfile.Company.Industry,
                            user.Location,
                            new List<string>());
                    }

                    return new MatchProfile(recruiterProfile?.JobTitle, null, null, null, user.Location, new List<string>());
                }

                var professionalProfile = await professionalProfileRepository.GetByUserIdAsync(user.Id);

                if (professionalProfile is null)
                {
                    return new MatchProfile(null, null, null, null, user.Location, new List<string>());
                }

                var experiences = await experienceRepository.GetByProfessionalProfileIdAsync(
                    new PageRequest { PageNumber = 1, PageSize = 50 }, false, professionalProfile.Id);

                var currentJob = experiences.Items.FirstOrDefault(e => e.IsCurrentJob && e.CompanyId is not null);

                var educations = await educationRepository.GetByProfessionalProfileIdAsync(
                    new PageRequest { PageNumber = 1, PageSize = 50 }, false, professionalProfile.Id);

                var institutions = educations.Items.Select(e => e.Institution).ToList();

                return new MatchProfile(
                    professionalProfile.HeadLine,
                    currentJob?.Company?.Name ?? experiences.Items.FirstOrDefault(e => e.IsCurrentJob)?.CompanyName,
                    currentJob?.CompanyId,
                    currentJob?.Company?.Industry,
                    user.Location,
                    institutions);
            }

            private record MatchProfile(
                string? Headline,
                string? CompanyName,
                Guid? CompanyId,
                string? Industry,
                string? Location,
                List<string> Institutions);
        }
    }

    public enum SuggestionFilter
    {
        All,
        SameCompany,
        SameIndustry,
        Alumni
    }

    public record SuggestionResponse(
        Guid UserId,
        string FirstName,
        string LastName,
        string? ProfilePictureUrl,
        string? Headline,
        string? CompanyName,
        string? Location,
        int MutualConnectionsCount,
        bool IsSameCompany,
        bool IsSameIndustry,
        bool IsAlumni,
        int Score);
}