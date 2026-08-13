using Application.Common.Pagenation;
using Application.Common.Repositories;
using Application.Common.Dtos;
using Domain.Enums;
using MediatR;

namespace Application.Queries
{
    public class GetPortfolioLinksByProfile
    {
        public record GetPortfolioLinksByProfileQuery(
            Guid ProfessionalProfileId,
            PageRequest PageRequest,
            bool UsePaging
            ) : IRequest<Result<PageResponse<GetPortfolioLinksByProfileResponse>>>;

        public class GetPortfolioLinksByProfileHandler(
            IPortfolioLinkRepository portfolioLinkRepository) : IRequestHandler<GetPortfolioLinksByProfileQuery, Result<PageResponse<GetPortfolioLinksByProfileResponse>>>
        {
            public async Task<Result<PageResponse<GetPortfolioLinksByProfileResponse>>> Handle(GetPortfolioLinksByProfileQuery request, CancellationToken cancellationToken)
            {
                var links = await portfolioLinkRepository.GetByProfessionalProfileIdAsync(
                    request.PageRequest,
                    request.UsePaging,
                    request.ProfessionalProfileId);

                var items = links.Items.Select(l => new GetPortfolioLinksByProfileResponse(
                    l.Id, l.Title, l.Url, l.LinkType, l.Description, l.ThumbnailUrl, l.ViewCount, l.ClickCount)).ToList();

                var response = new PageResponse<GetPortfolioLinksByProfileResponse>
                {
                    Items = items,
                    TotalCount = links.TotalCount,
                    PageNumber = links.PageNumber,
                    PageSize = links.PageSize
                };

                return Result<PageResponse<GetPortfolioLinksByProfileResponse>>.Success(response, "Portfolio links retrieved successfully");
            }
        }
    }
}