using Application.Common.Dtos;
using Application.Common.Repositories;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Commands.Messaging
{
    public class AddParticipant
    {
        public record AddParticipantCommand(Guid RequestingUserId, Guid ConversationId, Guid NewParticipantId) : IRequest<Result<string>>;

        public class AddParticipantHandler(
            IConversationRepository conversationRepository,
            IConversationParticipantRepository participantRepository,
            IUserConnectionRepository connectionRepository,
            IUserRepository userRepository,
            INotificationService notificationService,
            IUnitOfWork unitOfWork) : IRequestHandler<AddParticipantCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(AddParticipantCommand request, CancellationToken cancellationToken)
            {
                var conversation = await conversationRepository.GetByIdAsync(request.ConversationId);

                if (conversation is null)
                {
                    return Result<string>.Failure("Conversation not found");
                }

                if (!conversation.IsGroup)
                {
                    return Result<string>.Failure("You can't add people to a one-to-one conversation");
                }

                var requester = await participantRepository.GetByConversationAndUserAsync(request.ConversationId, request.RequestingUserId);

                if (requester is null)
                {
                    return Result<string>.Failure("You're not a participant of this conversation");
                }

                var connection = await connectionRepository.GetConnectionBetweenUsersAsync(request.RequestingUserId, request.NewParticipantId);

                if (connection is null || connection.ConnectionStatus != ConnectionStatus.Accepted)
                {
                    return Result<string>.Failure("You can only add people you're connected with");
                }

                var existingParticipant = await participantRepository.GetByConversationAndUserAsync(request.ConversationId, request.NewParticipantId);

                if (existingParticipant is not null)
                {
                    return Result<string>.Failure("This person is already in the group");
                }

                await participantRepository.AddAsync(new ConversationParticipant
                {
                    ConversationId = request.ConversationId,
                    UserId = request.NewParticipantId,
                    CreatedBy = request.RequestingUserId.ToString()
                });

                await unitOfWork.SaveAsync();

                var requesterUser = await userRepository.GetByIdAsync(request.RequestingUserId);
                var requesterName = requesterUser is not null ? $"{requesterUser.FirstName} {requesterUser.LastName}".Trim() : "Someone";

                await notificationService.SendNotificationAsync(
                    recipientUserId: request.NewParticipantId,
                    actorUserId: request.RequestingUserId,
                    actorName: requesterName,
                    actorAvatarUrl: requesterUser?.ProfilePictureUrl,
                    title: "Added to a group",
                    message: $"{requesterName} added you to \"{conversation.Title}\"",
                    type: NotificationType.Message,
                    sourceEntityType: NotificationSourceEntityType.Conversation,
                    sourceEntityId: request.ConversationId,
                    actionUrl: $"/messages.html?conversationId={request.ConversationId}",
                    createdBy: request.RequestingUserId.ToString());

                return Result<string>.Success(string.Empty, "Added to the group");
            }
        }
    }
}