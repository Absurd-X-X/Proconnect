using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.Commands.Networking
{
    public class RejectConnectionRequest
    {
        public record RejectConnectionRequestCommand(Guid UserId, Guid ConnectionId) : IRequest<Result<ConnectionResponse>>;

        public class RejectConnectionRequestHandler(
            IUserConnectionRepository connectionRepository,
            IUnitOfWork unitOfWork) : IRequestHandler<RejectConnectionRequestCommand, Result<ConnectionResponse>>
        {
            public async Task<Result<ConnectionResponse>> Handle(RejectConnectionRequestCommand request, CancellationToken cancellationToken)
            {
                var connection = await connectionRepository.GetByIdAsync(request.ConnectionId);

                if (connection is null)
                {
                    return Result<ConnectionResponse>.Failure("Connection request not found");
                }

                if (connection.RecieverId != request.UserId)
                {
                    return Result<ConnectionResponse>.Failure("You are not authorized to respond to this request");
                }

                if (connection.ConnectionStatus != ConnectionStatus.Pending)
                {
                    return Result<ConnectionResponse>.Failure("This request has already been responded to");
                }

                connection.ConnectionStatus = ConnectionStatus.Rejected;
                connection.DateUpdated = DateTime.UtcNow;

                connectionRepository.Update(connection);
                await unitOfWork.SaveAsync();

                return Result<ConnectionResponse>.Success(
                    new ConnectionResponse(connection.Id, connection.SenderId, connection.RecieverId, connection.ConnectionStatus, connection.DateCreated),
                    "Connection request rejected");
            }
        }
    }
}