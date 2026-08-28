using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Commands.Networking
{
    public class SendConnectionRequest
    {
        public record SendConnectionRequestCommand(Guid SenderId, Guid ReceiverId) : IRequest<Result<ConnectionResponse>>;

        public class SendConnectionRequestHandler(
            IUserConnectionRepository connectionRepository,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork) : IRequestHandler<SendConnectionRequestCommand, Result<ConnectionResponse>>
        {
            public async Task<Result<ConnectionResponse>> Handle(SendConnectionRequestCommand request, CancellationToken cancellationToken)
            {
                if (request.SenderId == request.ReceiverId)
                {
                    return Result<ConnectionResponse>.Failure("You cannot connect with yourself");
                }

                var receiver = await userRepository.GetByIdAsync(request.ReceiverId);

                if (receiver is null)
                {
                    return Result<ConnectionResponse>.Failure("User not found");
                }

                var existing = await connectionRepository.GetConnectionBetweenUsersAsync(request.SenderId, request.ReceiverId);

                if (existing is not null)
                {
                    if (existing.ConnectionStatus == ConnectionStatus.Accepted)
                    {
                        return Result<ConnectionResponse>.Failure("You are already connected with this user");
                    }

                    if (existing.ConnectionStatus == ConnectionStatus.Pending)
                    {
                        return Result<ConnectionResponse>.Failure("A connection request is already pending");
                    }

                    existing.SenderId = request.SenderId;
                    existing.RecieverId = request.ReceiverId;
                    existing.ConnectionStatus = ConnectionStatus.Pending;
                    existing.DateUpdated = DateTime.UtcNow;

                    connectionRepository.Update(existing);
                    await unitOfWork.SaveAsync();

                    return Result<ConnectionResponse>.Success(
                        new ConnectionResponse(existing.Id, existing.SenderId, existing.RecieverId, existing.ConnectionStatus, existing.DateCreated),
                        "Connection request sent");
                }

                var connection = new UserConnection
                {
                    SenderId = request.SenderId,
                    RecieverId = request.ReceiverId,
                    ConnectionStatus = ConnectionStatus.Pending,
                    CreatedBy = request.SenderId.ToString()
                };

                await connectionRepository.AddAsync(connection);
                await unitOfWork.SaveAsync();

                return Result<ConnectionResponse>.Success(
                    new ConnectionResponse(connection.Id, connection.SenderId, connection.RecieverId, connection.ConnectionStatus, connection.DateCreated),
                    "Connection request sent");
            }
        }
    }

    public record ConnectionResponse(
        Guid Id,
        Guid SenderId,
        Guid ReceiverId,
        ConnectionStatus ConnectionStatus,
        DateTime DateCreated);
}