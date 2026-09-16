using Application.Common.Dtos;
using Application.Common.Repositories;
using Application.Services.Interfaces;
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
            INotificationService notificationService,
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

                UserConnection connection;

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

                    connection = existing;
                }
                else
                {
                    connection = new UserConnection
                    {
                        SenderId = request.SenderId,
                        RecieverId = request.ReceiverId,
                        ConnectionStatus = ConnectionStatus.Pending,
                        CreatedBy = request.SenderId.ToString()
                    };

                    await connectionRepository.AddAsync(connection);
                    await unitOfWork.SaveAsync();
                }

                var sender = await userRepository.GetByIdAsync(request.SenderId);
                var senderName = sender is not null ? $"{sender.FirstName} {sender.LastName}".Trim() : "Someone";

                await notificationService.SendNotificationAsync(
                    recipientUserId: request.ReceiverId,
                    actorUserId: request.SenderId,
                    actorName: senderName,
                    actorAvatarUrl: sender?.ProfilePictureUrl,
                    title: "New connection request",
                    message: $"{senderName} sent you a connection request",
                    type: NotificationType.ConnectionRequest,
                    sourceEntityType: NotificationSourceEntityType.ConnectionRequest,
                    sourceEntityId: connection.Id,
                    actionUrl: "/connections.html?tab=requests",
                    createdBy: request.SenderId.ToString());

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