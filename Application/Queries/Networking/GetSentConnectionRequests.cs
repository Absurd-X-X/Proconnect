using Application.Common.Dtos;
using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.Queries.Networking
{
    public class GetSentConnectionRequests
    {
        public record GetSentConnectionRequestsQuery(Guid UserId, PageRequest PageRequest, bool UsePaging) : IRequest<Result<PageResponse<SentConnectionRequestResponse>>>;

        public class GetSentConnectionRequestsHandler(
            IUserConnectionRepository connectionRepository) : IRequestHandler<GetSentConnectionRequestsQuery, Result<PageResponse<SentConnectionRequestResponse>>>
        {
            public async Task<Result<PageResponse<SentConnectionRequestResponse>>> Handle(GetSentConnectionRequestsQuery request, CancellationToken cancellationToken)
            {
                var page = await connectionRepository.GetSentRequestsAsync(
                    request.UserId, ConnectionStatus.Pending, request.PageRequest, request.UsePaging);

                var items = page.Items.Select(c => new SentConnectionRequestResponse(
                    c.Id,
                    c.Reciever.Id,
                    c.Reciever.FirstName,
                    c.Reciever.LastName,
                    c.Reciever.ProfilePictureUrl,
                    c.DateCreated)).ToList();

                var response = new PageResponse<SentConnectionRequestResponse>
                {
                    Items = items,
                    TotalCount = page.TotalCount,
                    PageNumber = page.PageNumber,
                    PageSize = page.PageSize
                };

                return Result<PageResponse<SentConnectionRequestResponse>>.Success(response, "Sent requests retrieved successfully");
            }
        }
    }

    public record SentConnectionRequestResponse(
        Guid ConnectionId,
        Guid ReceiverId,
        string FirstName,
        string LastName,
        string? ProfilePictureUrl,
        DateTime DateCreated);
}