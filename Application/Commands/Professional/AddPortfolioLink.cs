using Application.Common.Dtos;
using Application.Common.Repositories;
using Application.Contract.Settings;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commands.Professional
{
    public class AddPortfolioLink
    {
        public record AddPortfolioLinkCommand(
            Guid ProfessionalProfileId,
            string Title,
            string Url,
            PortfolioLinkType LinkType,
            string? Description,
            IFormFile? Thumbnail,
            string CreatedBy
            ) : IRequest<Result<string>>;

        public class AddPortfolioLinkHandler(
            IProfessionalProfileRepository professionalProfileRepository,
            IPortfolioLinkRepository portfolioLinkRepository,
            IFileStorage fileStorage,
            IUnitOfWork unitOfWork) : IRequestHandler<AddPortfolioLinkCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(AddPortfolioLinkCommand request, CancellationToken cancellationToken)
            {
                var profile = await professionalProfileRepository.GetByIdAsync(request.ProfessionalProfileId);

                if (profile is null)
                    return Result<string>.Failure("Professional profile not found");

                var link = new PortfolioLink
                {
                    ProfessionalProfileId = request.ProfessionalProfileId,
                    Title = request.Title,
                    Url = request.Url,
                    LinkType = request.LinkType,
                    Description = request.Description,
                    CreatedBy = request.CreatedBy
                };

                if (request.Thumbnail is not null && request.Thumbnail.Length > 0)
                {
                    var uploadResult = await fileStorage.UploadAsync(
                        request.Thumbnail,
                        "proconnect/portfolio-thumbnails",
                        cancellationToken);

                    link.ThumbnailUrl = uploadResult.Url;

                    link.ThumbnailPublicId = uploadResult.PublicId;
                }

                await portfolioLinkRepository.CreateAsync(link);

                await unitOfWork.SaveAsync();

                return Result<string>.Success("Portfolio link added successfully", "added");
            }
        }
    }
}