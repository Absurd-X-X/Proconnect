using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Commands.Review
{
    public class CreateCompanyReview
    {
        public record CreateCompanyReviewCommand(
            Guid CompanyId,
            Guid ReviewerId,
            int OverallRating,
            int WorkLifeBalanceRating,
            int CompensationRating,
            int JobSecurityRating,
            int ManagementRating,
            int CultureRating,
            string Title,
            string Content,
            string JobTitle,
            string? Location,
            bool IsCurrentEmployee,
            int? YearsAtCompany,
            string CreatedBy
        ) : IRequest<Result<string>>;

        public class CreateCompanyReviewHandler(
            ICompanyReviewRepository reviewRepository,
            IUnitOfWork unitOfWork)
            : IRequestHandler<CreateCompanyReviewCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(
                CreateCompanyReviewCommand request,
                CancellationToken cancellationToken)
            {
                var exists = await reviewRepository.ExistsAsync(request.CompanyId, request.ReviewerId);

                if (exists)
                    return Result<string>.Failure("You've already reviewed this company");

                foreach (var rating in new[]
                {
                    request.OverallRating, request.WorkLifeBalanceRating, request.CompensationRating,
                    request.JobSecurityRating, request.ManagementRating, request.CultureRating
                })
                {
                    if (rating < 1 || rating > 5)
                        return Result<string>.Failure("Ratings must be between 1 and 5");
                }

                var review = new CompanyReview
                {
                    CompanyId = request.CompanyId,

                    ReviewerId = request.ReviewerId,

                    OverallRating = request.OverallRating,

                    WorkLifeBalanceRating = request.WorkLifeBalanceRating,

                    CompensationRating = request.CompensationRating,

                    JobSecurityRating = request.JobSecurityRating,

                    ManagementRating = request.ManagementRating,

                    CultureRating = request.CultureRating,

                    Title = request.Title,

                    Content = request.Content,

                    JobTitle = request.JobTitle,

                    Location = request.Location,

                    IsCurrentEmployee = request.IsCurrentEmployee,

                    YearsAtCompany = request.YearsAtCompany,

                    CreatedBy = request.CreatedBy,

                    DateCreated = DateTime.UtcNow
                };

                await reviewRepository.AddAsync(review);

                await unitOfWork.SaveAsync();

                return Result<string>.Success(review.Id.ToString(), "Review submitted successfully");
            }
        }
    }
}