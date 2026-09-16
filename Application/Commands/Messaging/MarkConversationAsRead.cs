using Application.Common.Dtos;
using Application.Common.Repositories;
using Application.Services.Interfaces;
using Domain.Enums;
using MediatR;

namespace Application.Commands.Messaging
{
    public class MarkConversationRead
    {
        public record MarkConversationReadCommand(Guid UserId, Guid ConversationId) : IRequest<Result<string>>;

        public class MarkConversationReadHandler(
            IConversationParticipantRepository participantRepository,
            INotificationService notificationService,
            IUnitOfWork unitOfWork) : IRequestHandler<MarkConversationReadCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(MarkConversationReadCommand request, CancellationToken cancellationToken)
            {
                var participant = await participantRepository.GetByConversationAndUserAsync(request.ConversationId, request.UserId);

                if (participant is null)
                {
                    return Result<string>.Failure("You're not a participant of this conversation");
                }

                participant.LastReadAt = DateTime.UtcNow;
                participantRepository.Update(participant);
                await unitOfWork.SaveAsync();

                await notificationService.MarkAsReadBySourceAsync(
                    request.UserId, NotificationSourceEntityType.Conversation, request.ConversationId);

                return Result<string>.Success(string.Empty, "Marked as read");
            }
        }
    }
}