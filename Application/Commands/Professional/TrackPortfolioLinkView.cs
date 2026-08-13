using Application.Common.Repositories;
using Application.Common.Dtos;
using MediatR;

namespace Application.Commands.Professional
{
    public class TrackPortfolioLinkView
    {
        public record TrackPortfolioLinkViewCommand(Guid Id) : IRequest<Result<string>>;

        public class TrackPortfolioLinkViewHandler(
            IPortfolioLinkRepository portfolioLinkRepository,
            IUnitOfWork unitOfWork) : IRequestHandler<TrackPortfolioLinkViewCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(TrackPortfolioLinkViewCommand request, CancellationToken cancellationToken)
            {
                var link = await portfolioLinkRepository.GetByIdAsync(request.Id);

                if (link is null)
                    return Result<string>.Failure("Portfolio link not found");

                link.ViewCount += 1;

                portfolioLinkRepository.UpdateAsync(link);

                await unitOfWork.SaveAsync();

                return Result<string>.Success("View recorded", "tracked");
            }
        }
    }
}