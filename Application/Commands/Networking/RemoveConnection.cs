using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.Commands.Networking
{
    public class RemoveConnection
    {
        public record RemoveConnectionCommand(Guid UserId, Guid ConnectionId) : IRequest<Result<string>>;

        public class RemoveConnectionHandler(
            IUserConnectionRepository connectionRepository,
            IUnitOfWork unitOfWork) : IRequestHandler<RemoveConnectionCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(RemoveConnectionCommand request, CancellationToken cancellationToken)
            {
                var connection = await connectionRepository.GetByIdAsync(request.ConnectionId);

                if (connection is null)
                {
                    return Result<string>.Failure("Connection not found");
                }

                if (connection.SenderId != request.UserId && connection.RecieverId != request.UserId)
                {
                    return Result<string>.Failure("You are not authorized to remove this connection");
                }

                if (connection.ConnectionStatus != ConnectionStatus.Accepted)
                {
                    return Result<string>.Failure("This connection cannot be removed");
                }

                connection.IsDeleted = true;
                connection.DateUpdated = DateTime.UtcNow;

                connectionRepository.Update(connection);
                await unitOfWork.SaveAsync();

                return Result<string>.Success(string.Empty, "Connection removed");
            }
        }
    }
}