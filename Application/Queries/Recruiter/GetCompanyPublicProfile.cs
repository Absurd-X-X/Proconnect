using Application.Common.Dtos;
using Application.Common.Repositories;
using MediatR;

namespace Application.Queries.Recruiter
{
    public class GetCompanyPublicProfile
    {
        public record GetCompanyPublicProfileQuery(Guid CompanyId) : IRequest<Result<GetCompanyPublicProfileResponse>>;

        public class GetCompanyPublicProfileHandler(
            ICompanyRepository companyRepository,
            ICompanyReviewRepository reviewRepository)
            : IRequestHandler<GetCompanyPublicProfileQuery, Result<GetCompanyPublicProfileResponse>>
        {
            public async Task<Result<GetCompanyPublicProfileResponse>> Handle(
                GetCompanyPublicProfileQuery request,
                CancellationToken cancellationToken)
            {
                var company = await companyRepository.GetByIdWithDetailsAsync(request.CompanyId);

                if (company is null)
                    return Result<GetCompanyPublicProfileResponse>.Failure("Company not found");

                var reviews = await reviewRepository.GetAllByCompanyIdAsync(request.CompanyId);

                var avgRating = reviews.Count > 0
                    ? Math.Round(reviews.Average(r => r.OverallRating), 1)
                    : 0;

                var activeJobCount = company.Jobs.Count(j => j.IsActive);

                var response = new GetCompanyPublicProfileResponse(
                    company.Id,
                    company.Name,
                    company.Industry,
                    company.Description,
                    company.Website,
                    company.LogoUrl,
                    company.IsVerified,
                    company.FoundedYear,
                    company.CompanySize,
                    company.CompanyType,
                    company.Headquarters,
                    company.LinkedInUrl,
                    company.TwitterUrl,
                    company.FacebookUrl,
                    company.InstagramUrl,
                    company.Locations,
                    company.Strengths,
                    activeJobCount,
                    reviews.Count,
                    avgRating);

                return Result<GetCompanyPublicProfileResponse>.Success(response, "Company profile retrieved successfully");
            }
        }
    }

    public record GetCompanyPublicProfileResponse(
        Guid Id,
        string Name,
        string Industry,
        string Description,
        string? Website,
        string? LogoUrl,
        bool IsVerified,
        int? FoundedYear,
        string CompanySize,
        string CompanyType,
        string? Headquarters,
        string? LinkedInUrl,
        string? TwitterUrl,
        string? FacebookUrl,
        string? InstagramUrl,
        string? Locations,
        string? Strengths,
        int ActiveJobCount,
        int ReviewCount,
        double AverageRating);
}