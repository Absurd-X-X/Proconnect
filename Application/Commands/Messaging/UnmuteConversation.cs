using Application.Common.Dtos;
using Application.Common.Repositories;
using MediatR;

namespace Application.Commands.Messaging
{
    public class UnmuteConversation
    {
        public record UnmuteConversationCommand(Guid UserId, Guid ConversationId) : IRequest<Result<string>>;

        public class UnmuteConversationHandler(
            IConversationParticipantRepository participantRepository,
            IUnitOfWork unitOfWork) : IRequestHandler<UnmuteConversationCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(UnmuteConversationCommand request, CancellationToken cancellationToken)
            {
                var participant = await participantRepository.GetByConversationAndUserAsync(request.ConversationId, request.UserId);

                if (participant is null)
                {
                    return Result<string>.Failure("You're not a participant of this conversation");
                }

                participant.IsMuted = false;
                participantRepository.Update(participant);
                await unitOfWork.SaveAsync();

                return Result<string>.Success(string.Empty, "Conversation unmuted");
            }
        }
    }
}