using Application.Common.Dtos;
using Application.Common.Pagenation;
using Application.Common.Repositories;
using MediatR;

namespace Application.Queries.Company
{
    public class GetCompanyReviews
    {
        public record GetCompanyReviewsQuery(
            Guid CompanyId,
            PageRequest PageRequest,
            bool UsePaging
        ) : IRequest<Result<PageResponse<GetCompanyReviewsResponse>>>;

        public class GetCompanyReviewsHandler(
            ICompanyReviewRepository reviewRepository)
            : IRequestHandler<GetCompanyReviewsQuery, Result<PageResponse<GetCompanyReviewsResponse>>>
        {
            public async Task<Result<PageResponse<GetCompanyReviewsResponse>>> Handle(
                GetCompanyReviewsQuery request,
                CancellationToken cancellationToken)
            {
                var reviews = await reviewRepository.GetByCompanyIdAsync(
                    request.CompanyId, request.PageRequest, request.UsePaging);

                var response = new PageResponse<GetCompanyReviewsResponse>
                {
                    Items = reviews.Items.Select(r => new GetCompanyReviewsResponse(
                        r.Id,
                        r.Reviewer.FirstName,
                        r.Reviewer.LastName,
                        r.JobTitle,
                        r.Location,
                        r.IsCurrentEmployee,
                        r.YearsAtCompany,
                        r.OverallRating,
                        r.Title,
                        r.Content,
                        r.HelpfulCount,
                        r.DateCreated)).ToList(),
                    TotalCount = reviews.TotalCount,
                    PageNumber = reviews.PageNumber,
                    PageSize = reviews.PageSize
                };

                return Result<PageResponse<GetCompanyReviewsResponse>>.Success(response, "Reviews retrieved successfully");
            }
        }
    }

    public record GetCompanyReviewsResponse(
        Guid Id,
        string ReviewerFirstName,
        string ReviewerLastName,
        string JobTitle,
        string? Location,
        bool IsCurrentEmployee,
        int? YearsAtCompany,
        int OverallRating,
        string Title,
        string Content,
        int HelpfulCount,
        DateTime DateCreated);
}