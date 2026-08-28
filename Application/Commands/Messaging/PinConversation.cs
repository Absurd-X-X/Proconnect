using Application.Common.Dtos;
using Application.Common.Repositories;
using MediatR;

namespace Application.Commands.Messaging
{
    public class PinConversation
    {
        public record PinConversationCommand(Guid UserId, Guid ConversationId) : IRequest<Result<string>>;

        public class PinConversationHandler(
            IConversationParticipantRepository participantRepository,
            IUnitOfWork unitOfWork) : IRequestHandler<PinConversationCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(PinConversationCommand request, CancellationToken cancellationToken)
            {
                var participant = await participantRepository.GetByConversationAndUserAsync(request.ConversationId, request.UserId);

                if (participant is null)
                {
                    return Result<string>.Failure("You're not a participant of this conversation");
                }

                participant.IsPinned = true;
                participantRepository.Update(participant);
                await unitOfWork.SaveAsync();

                return Result<string>.Success(string.Empty, "Conversation pinned");
            }
        }
    }
}