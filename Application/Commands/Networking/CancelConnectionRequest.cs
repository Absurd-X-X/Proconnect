using Application.Commands.Networking;
using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.Commands.Connections
{
    public class CancelConnectionRequest
    {
        public record CancelConnectionRequestCommand(Guid UserId, Guid ConnectionId) : IRequest<Result<ConnectionResponse>>;

        public class CancelConnectionRequestHandler(
            IUserConnectionRepository connectionRepository,
            IUnitOfWork unitOfWork) : IRequestHandler<CancelConnectionRequestCommand, Result<ConnectionResponse>>
        {
            public async Task<Result<ConnectionResponse>> Handle(CancelConnectionRequestCommand request, CancellationToken cancellationToken)
            {
                var connection = await connectionRepository.GetByIdAsync(request.ConnectionId);

                if (connection is null)
                {
                    return Result<ConnectionResponse>.Failure("Connection request not found");
                }

                if (connection.SenderId != request.UserId)
                {
                    return Result<ConnectionResponse>.Failure("You are not authorized to cancel this request");
                }

                if (connection.ConnectionStatus != ConnectionStatus.Pending)
                {
                    return Result<ConnectionResponse>.Failure("Only pending requests can be cancelled");
                }

                connection.ConnectionStatus = ConnectionStatus.Cancelled;
                connection.DateUpdated = DateTime.UtcNow;

                connectionRepository.Update(connection);
                await unitOfWork.SaveAsync();

                return Result<ConnectionResponse>.Success(
                    new ConnectionResponse(connection.Id, connection.SenderId, connection.RecieverId, connection.ConnectionStatus, connection.DateCreated),
                    "Connection request cancelled");
            }
        }
    }
}