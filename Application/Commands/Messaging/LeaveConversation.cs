using Application.Common.Dtos;
using Application.Common.Repositories;
using Application.Services.Interfaces;
using Domain.Enums;
using MediatR;

namespace Application.Commands.Messaging
{
    public class LeaveConversation
    {
        public record LeaveConversationCommand(Guid UserId, Guid ConversationId) : IRequest<Result<string>>;

        public class LeaveConversationHandler(
            IConversationParticipantRepository participantRepository,
            IUserRepository userRepository,
            INotificationService notificationService,
            IUnitOfWork unitOfWork) : IRequestHandler<LeaveConversationCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(LeaveConversationCommand request, CancellationToken cancellationToken)
            {
                var participant = await participantRepository.GetByConversationAndUserAsync(request.ConversationId, request.UserId);

                if (participant is null)
                {
                    return Result<string>.Failure("You're not a participant of this conversation");
                }

                var remainingParticipants = await participantRepository.GetByConversationIdAsync(request.ConversationId);

                participant.IsDeleted = true;
                participantRepository.Update(participant);
                await unitOfWork.SaveAsync();

                var leavingUser = await userRepository.GetByIdAsync(request.UserId);
                var leavingUserName = leavingUser is not null ? $"{leavingUser.FirstName} {leavingUser.LastName}".Trim() : "Someone";

                foreach (var member in remainingParticipants.Where(p => p.UserId != request.UserId))
                {
                    await notificationService.SendNotificationAsync(
                        recipientUserId: member.UserId,
                        actorUserId: request.UserId,
                        actorName: leavingUserName,
                        actorAvatarUrl: leavingUser?.ProfilePictureUrl,
                        title: "Someone left the group",
                        message: $"{leavingUserName} left the group",
                        type: NotificationType.Message,
                        sourceEntityType: NotificationSourceEntityType.Conversation,
                        sourceEntityId: request.ConversationId,
                        actionUrl: $"/messages.html?conversationId={request.ConversationId}",
                        createdBy: request.UserId.ToString());
                }

                return Result<string>.Success(string.Empty, "You left the conversation");
            }
        }
    }
}