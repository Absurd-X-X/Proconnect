using Application.Common.Dtos;
using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.Queries.Connections
{
    public class GetMyConnections
    {
        public record GetMyConnectionsQuery(Guid UserId, PageRequest PageRequest, bool UsePaging) : IRequest<Result<PageResponse<ConnectionListItemResponse>>>;

        public class GetMyConnectionsHandler(
            IUserConnectionRepository connectionRepository) : IRequestHandler<GetMyConnectionsQuery, Result<PageResponse<ConnectionListItemResponse>>>
        {
            public async Task<Result<PageResponse<ConnectionListItemResponse>>> Handle(GetMyConnectionsQuery request, CancellationToken cancellationToken)
            {
                var page = await connectionRepository.GetUserConnectionsAsync(request.UserId, request.PageRequest, request.UsePaging);

                var items = page.Items.Select(c =>
                {
                    var otherUser = c.SenderId == request.UserId ? c.Reciever : c.Sender;

                    return new ConnectionListItemResponse(
                        c.Id,
                        otherUser.Id,
                        otherUser.FirstName,
                        otherUser.LastName,
                        otherUser.ProfilePictureUrl,
                        c.ConnectionStatus,
                        c.DateUpdated);
                }).ToList();

                var response = new PageResponse<ConnectionListItemResponse>
                {
                    Items = items,
                    TotalCount = page.TotalCount,
                    PageNumber = page.PageNumber,
                    PageSize = page.PageSize
                };

                return Result<PageResponse<ConnectionListItemResponse>>.Success(response, "Connections retrieved successfully");
            }
        }
    }

    public record ConnectionListItemResponse(
        Guid ConnectionId,
        Guid UserId,
        string FirstName,
        string LastName,
        string? ProfilePictureUrl,
        ConnectionStatus Status,
        DateTime DateUpdated);
}