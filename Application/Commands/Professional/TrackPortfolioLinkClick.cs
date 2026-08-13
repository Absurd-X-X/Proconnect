using Application.Common.Repositories;
using Application.Common.Dtos;
using MediatR;

namespace Application.Commands.Professional
{
    public class TrackPortfolioLinkClick
    {
        public record TrackPortfolioLinkClickCommand(Guid Id) : IRequest<Result<string>>;

        public class TrackPortfolioLinkClickHandler(
            IPortfolioLinkRepository portfolioLinkRepository,
            IUnitOfWork unitOfWork) : IRequestHandler<TrackPortfolioLinkClickCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(TrackPortfolioLinkClickCommand request, CancellationToken cancellationToken)
            {
                var link = await portfolioLinkRepository.GetByIdAsync(request.Id);

                if (link is null)
                    return Result<string>.Failure("Portfolio link not found");

                link.ClickCount += 1;

                portfolioLinkRepository.UpdateAsync(link);

                await unitOfWork.SaveAsync();

                return Result<string>.Success("Click recorded", "tracked");
            }
        }
    }
}