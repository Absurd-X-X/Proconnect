using Application.Common.Dtos;
using Application.Common.Repositories;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Commands.Messaging
{
    public class CreateGroupConversation
    {
        public record CreateGroupConversationCommand(Guid UserId, string Title, List<Guid> ParticipantIds)
            : IRequest<Result<ConversationResponse>>;

        public class CreateGroupConversationHandler(
            IConversationRepository conversationRepository,
            IConversationParticipantRepository participantRepository,
            IUserConnectionRepository connectionRepository,
            IUserRepository userRepository,
            INotificationService notificationService,
            IUnitOfWork unitOfWork) : IRequestHandler<CreateGroupConversationCommand, Result<ConversationResponse>>
        {
            public async Task<Result<ConversationResponse>> Handle(CreateGroupConversationCommand request, CancellationToken cancellationToken)
            {
                if (string.IsNullOrWhiteSpace(request.Title))
                {
                    return Result<ConversationResponse>.Failure("A group name is required");
                }

                var invitees = request.ParticipantIds.Distinct().Where(id => id != request.UserId).ToList();

                if (invitees.Count == 0)
                {
                    return Result<ConversationResponse>.Failure("Add at least one other person to the group");
                }

                foreach (var inviteeId in invitees)
                {
                    var connection = await connectionRepository.GetConnectionBetweenUsersAsync(request.UserId, inviteeId);

                    if (connection is null || connection.ConnectionStatus != ConnectionStatus.Accepted)
                    {
                        return Result<ConversationResponse>.Failure("You can only add people you're connected with to a group");
                    }
                }

                var conversation = new Conversation
                {
                    IsGroup = true,
                    Title = request.Title.Trim(),
                    CreatedBy = request.UserId.ToString()
                };

                await conversationRepository.AddAsync(conversation);
                await unitOfWork.SaveAsync();

                await participantRepository.AddAsync(new ConversationParticipant
                {
                    ConversationId = conversation.Id,
                    UserId = request.UserId,
                    CreatedBy = request.UserId.ToString()
                });

                foreach (var inviteeId in invitees)
                {
                    await participantRepository.AddAsync(new ConversationParticipant
                    {
                        ConversationId = conversation.Id,
                        UserId = inviteeId,
                        CreatedBy = request.UserId.ToString()
                    });
                }

                await unitOfWork.SaveAsync();

                var creator = await userRepository.GetByIdAsync(request.UserId);
                var creatorName = creator is not null ? $"{creator.FirstName} {creator.LastName}".Trim() : "Someone";

                foreach (var inviteeId in invitees)
                {
                    await notificationService.SendNotificationAsync(
                        recipientUserId: inviteeId,
                        actorUserId: request.UserId,
                        actorName: creatorName,
                        actorAvatarUrl: creator?.ProfilePictureUrl,
                        title: "Added to a group",
                        message: $"{creatorName} added you to \"{conversation.Title}\"",
                        type: NotificationType.Message,
                        sourceEntityType: NotificationSourceEntityType.Conversation,
                        sourceEntityId: conversation.Id,
                        actionUrl: $"/messages.html?conversationId={conversation.Id}",
                        createdBy: request.UserId.ToString());
                }

                return Result<ConversationResponse>.Success(
                    new ConversationResponse(conversation.Id, conversation.IsGroup, conversation.Title),
                    "Group created");
            }
        }
    }
}