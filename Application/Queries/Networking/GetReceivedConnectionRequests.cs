using Application.Common.Dtos;
using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.Queries.Networking
{
    public class GetReceivedConnectionRequests
    {
        public record GetReceivedConnectionRequestsQuery(Guid UserId, PageRequest PageRequest, bool UsePaging) : IRequest<Result<PageResponse<ConnectionRequestResponse>>>;

        public class GetReceivedConnectionRequestsHandler(
            IUserConnectionRepository connectionRepository) : IRequestHandler<GetReceivedConnectionRequestsQuery, Result<PageResponse<ConnectionRequestResponse>>>
        {
            public async Task<Result<PageResponse<ConnectionRequestResponse>>> Handle(GetReceivedConnectionRequestsQuery request, CancellationToken cancellationToken)
            {
                var page = await connectionRepository.GetReceivedRequestsAsync(
                    request.UserId, ConnectionStatus.Pending, request.PageRequest, request.UsePaging);

                var items = page.Items.Select(c => new ConnectionRequestResponse(
                    c.Id,
                    c.Sender.Id,
                    c.Sender.FirstName,
                    c.Sender.LastName,
                    c.Sender.ProfilePictureUrl,
                    c.DateCreated)).ToList();

                var response = new PageResponse<ConnectionRequestResponse>
                {
                    Items = items,
                    TotalCount = page.TotalCount,
                    PageNumber = page.PageNumber,
                    PageSize = page.PageSize
                };

                return Result<PageResponse<ConnectionRequestResponse>>.Success(response, "Pending requests retrieved successfully");
            }
        }
    }

    public record ConnectionRequestResponse(
        Guid ConnectionId,
        Guid SenderId,
        string FirstName,
        string LastName,
        string? ProfilePictureUrl,
        DateTime DateCreated);
}