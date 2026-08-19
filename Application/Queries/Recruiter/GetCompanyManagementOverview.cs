using Application.Common.Dtos;
using Application.Common.Repositories;
using MediatR;

namespace Application.Queries
{
    public class GetCompanyManagementOverview
    {
        public record ManagementOverviewResponse(
            Guid CompanyId,
            string Name,
            string? LogoUrl,
            bool IsVerified,
            int TeamMemberCount,
            int OpenPositionCount,
            int ActiveCandidateCount
        );

        public record GetCompanyManagementOverviewQuery(Guid RequestingUserId) : IRequest<Result<ManagementOverviewResponse>>;

        public class GetCompanyManagementOverviewHandler(
            ICompanyRepository companyRepository,
            IRecruiterProfileRepository recruiterProfileRepository)
            : IRequestHandler<GetCompanyManagementOverviewQuery, Result<ManagementOverviewResponse>>
        {
            public async Task<Result<ManagementOverviewResponse>> Handle(
                GetCompanyManagementOverviewQuery request,
                CancellationToken cancellationToken)
            {
                var requestingProfile = await recruiterProfileRepository
                    .GetByUserIdAsync(request.RequestingUserId);

                if (requestingProfile is null || requestingProfile.CompanyId is null)
                {
                    return Result<ManagementOverviewResponse>.Failure("You are not linked to a company");
                }

                var company = await companyRepository.GetByIdWithDetailsAsync(requestingProfile.CompanyId.Value);

                if (company is null)
                {
                    return Result<ManagementOverviewResponse>.Failure("Company not found");
                }

                var activeCandidateCount = company.Jobs
                    .SelectMany(j => j.JobApplications)
                    .Count();

                var result = new ManagementOverviewResponse(
                    company.Id,
                    company.Name,
                    company.LogoUrl,
                    company.IsVerified,
                    company.RecruiterProfiles.Count,
                    company.Jobs.Count,
                    activeCandidateCount);

                return Result<ManagementOverviewResponse>.Success(result, "Overview retrieved");
            }
        }
    }
}