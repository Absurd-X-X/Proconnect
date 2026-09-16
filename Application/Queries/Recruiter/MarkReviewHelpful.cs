using Application.Common.Dtos;
using Application.Common.Repositories;
using MediatR;

namespace Application.Queries.Recruiter
{
    public class MarkReviewHelpful
    {
        public record MarkReviewHelpfulCommand(Guid ReviewId) : IRequest<Result<string>>;

        public class MarkReviewHelpfulHandler(
            ICompanyReviewRepository reviewRepository,
            IUnitOfWork unitOfWork)
            : IRequestHandler<MarkReviewHelpfulCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(
                MarkReviewHelpfulCommand request,
                CancellationToken cancellationToken)
            {
                var review = await reviewRepository.GetByIdAsync(request.ReviewId);

                if (review is null)
                    return Result<string>.Failure("Review not found");

                reviewRepository.IncrementHelpful(review);

                await unitOfWork.SaveAsync();

                return Result<string>.Success(review.HelpfulCount.ToString(), "Marked as helpful");
            }
        }
    }
}