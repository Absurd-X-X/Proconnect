using Application.Common.Dtos;
using Application.Common.Repositories;
using MediatR;

namespace Application.Queries.Company
{
    public class GetPeopleYouMightKnowAtCompany
    {
        public record GetPeopleYouMightKnowAtCompanyQuery(
            Guid CompanyId,
            Guid CurrentUserId,
            int Take
        ) : IRequest<Result<List<GetPeopleYouMightKnowResponse>>>;

        public class GetPeopleYouMightKnowAtCompanyHandler(
            ICompanyRepository companyRepository,
            IUserConnectionRepository userConnectionRepository)
            : IRequestHandler<GetPeopleYouMightKnowAtCompanyQuery, Result<List<GetPeopleYouMightKnowResponse>>>
        {
            public async Task<Result<List<GetPeopleYouMightKnowResponse>>> Handle(
                GetPeopleYouMightKnowAtCompanyQuery request,
                CancellationToken cancellationToken)
            {
                var company = await companyRepository.GetByIdWithDetailsAsync(request.CompanyId);

                if (company is null)
                    return Result<List<GetPeopleYouMightKnowResponse>>.Failure("Company not found");

                var relatedUserIds = await userConnectionRepository.GetRelatedUserIdsAsync(request.CurrentUserId);

                var candidates = company.RecruiterProfiles
                    .Where(r => r.UserId != request.CurrentUserId && !relatedUserIds.Contains(r.UserId))
                    .Take(request.Take * 2)
                    .ToList();

                var results = new List<GetPeopleYouMightKnowResponse>();

                foreach (var recruiter in candidates.Take(request.Take))
                {
                    var mutualCount = await userConnectionRepository.GetMutualConnectionsCountAsync(
                        request.CurrentUserId, recruiter.UserId);

                    results.Add(new GetPeopleYouMightKnowResponse(
                        recruiter.UserId,
                        recruiter.User.FirstName,
                        recruiter.User.LastName,
                        recruiter.User.ProfilePictureUrl,
                        mutualCount));
                }

                return Result<List<GetPeopleYouMightKnowResponse>>.Success(results, "Suggestions retrieved successfully");
            }
        }
    }

    public record GetPeopleYouMightKnowResponse(
        Guid UserId,
        string FirstName,
        string LastName,
        string? ProfilePictureUrl,
        int MutualConnectionCount);
}