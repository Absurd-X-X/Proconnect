using Application.Common.Dtos;
using Application.Common.Repositories;
using Application.Contract.Settings;
using MediatR;

namespace Application.Commands.Professional
{
    public class DeletePortfolioLink
    {
        public record DeletePortfolioLinkCommand(Guid Id, bool DeletePermanently) : IRequest<Result<string>>;

        public class DeletePortfolioLinkHandler(
            IPortfolioLinkRepository portfolioLinkRepository,
            IFileStorage fileStorage,
            IUnitOfWork unitOfWork) : IRequestHandler<DeletePortfolioLinkCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(DeletePortfolioLinkCommand request, CancellationToken cancellationToken)
            {
                var link = await portfolioLinkRepository.GetByIdAsync(request.Id);

                if (link is null)
                    return Result<string>.Failure("Portfolio link not found");

                if (request.DeletePermanently)
                {
                    if (!string.IsNullOrWhiteSpace(link.ThumbnailPublicId))
                    {
                        await fileStorage.DeleteAsync(link.ThumbnailPublicId, cancellationToken);
                    }

                    portfolioLinkRepository.Delete(link);
                }
                else
                {
                    link.IsDeleted = true;

                    portfolioLinkRepository.UpdateAsync(link);
                }

                await unitOfWork.SaveAsync();

                return Result<string>.Success("Portfolio link deleted successfully", "deleted");
            }
        }
    }
}