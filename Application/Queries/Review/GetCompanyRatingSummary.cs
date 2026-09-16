using Application.Common.Dtos;
using Application.Common.Repositories;
using MediatR;

namespace Application.Queries.Company
{
    public class GetCompanyRatingSummary
    {
        public record GetCompanyRatingSummaryQuery(Guid CompanyId) : IRequest<Result<GetCompanyRatingSummaryResponse>>;

        public class GetCompanyRatingSummaryHandler(
            ICompanyReviewRepository reviewRepository)
            : IRequestHandler<GetCompanyRatingSummaryQuery, Result<GetCompanyRatingSummaryResponse>>
        {
            public async Task<Result<GetCompanyRatingSummaryResponse>> Handle(
                GetCompanyRatingSummaryQuery request,
                CancellationToken cancellationToken)
            {
                var reviews = await reviewRepository.GetAllByCompanyIdAsync(request.CompanyId);

                if (reviews.Count == 0)
                {
                    return Result<GetCompanyRatingSummaryResponse>.Success(
                        new GetCompanyRatingSummaryResponse(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0),
                        "No reviews yet");
                }

                double Avg(Func<Domain.Entities.CompanyReview, int> selector) => reviews.Average(r => selector(r));

                var response = new GetCompanyRatingSummaryResponse(
                    reviews.Count,
                    Math.Round(Avg(r => r.OverallRating), 1),
                    reviews.Count(r => r.OverallRating == 5),
                    reviews.Count(r => r.OverallRating == 4),
                    reviews.Count(r => r.OverallRating == 3),
                    reviews.Count(r => r.OverallRating == 2),
                    reviews.Count(r => r.OverallRating == 1),
                    Math.Round(Avg(r => r.WorkLifeBalanceRating), 1),
                    Math.Round(Avg(r => r.CompensationRating), 1),
                    Math.Round(Avg(r => r.JobSecurityRating), 1),
                    Math.Round(Avg(r => r.ManagementRating), 1),
                    Math.Round(Avg(r => r.CultureRating), 1));

                return Result<GetCompanyRatingSummaryResponse>.Success(response, "Rating summary retrieved successfully");
            }
        }
    }

    public record GetCompanyRatingSummaryResponse(
        int TotalReviews,
        double OverallAverage,
        int FiveStarCount,
        int FourStarCount,
        int ThreeStarCount,
        int TwoStarCount,
        int OneStarCount,
        double WorkLifeBalanceAvg,
        double CompensationAvg,
        double JobSecurityAvg,
        double ManagementAvg,
        double CultureAvg);
}