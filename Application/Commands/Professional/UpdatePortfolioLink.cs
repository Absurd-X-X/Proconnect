using Application.Common.Dtos;
using Application.Common.Repositories;
using Application.Contract.Settings;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commands.Professional
{
    public class UpdatePortfolioLink
    {
        public record UpdatePortfolioLinkCommand(
            Guid Id,
            string Title,
            string Url,
            PortfolioLinkType LinkType,
            string? Description,
            IFormFile? Thumbnail
            ) : IRequest<Result<string>>;

        public class UpdatePortfolioLinkHandler(
            IPortfolioLinkRepository portfolioLinkRepository,
            IFileStorage fileStorage,
            IUnitOfWork unitOfWork) : IRequestHandler<UpdatePortfolioLinkCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(UpdatePortfolioLinkCommand request, CancellationToken cancellationToken)
            {
                var link = await portfolioLinkRepository.GetByIdAsync(request.Id);

                if (link is null)
                    return Result<string>.Failure("Portfolio link not found");

                link.Title = request.Title;

                link.Url = request.Url;

                link.LinkType = request.LinkType;

                link.Description = request.Description;

                if (request.Thumbnail is not null && request.Thumbnail.Length > 0)
                {
                    if (!string.IsNullOrWhiteSpace(link.ThumbnailPublicId))
                    {
                        await fileStorage.DeleteAsync(link.ThumbnailPublicId, cancellationToken);
                    }

                    var uploadResult = await fileStorage.UploadAsync(
                        request.Thumbnail,
                        "proconnect/portfolio-thumbnails",
                        cancellationToken);

                    link.ThumbnailUrl = uploadResult.Url;

                    link.ThumbnailPublicId = uploadResult.PublicId;
                }

                portfolioLinkRepository.UpdateAsync(link);

                await unitOfWork.SaveAsync();

                return Result<string>.Success("Portfolio link updated successfully", "updated");
            }
        }
    }
}