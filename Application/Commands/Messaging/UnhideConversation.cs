using Application.Common.Dtos;
using Application.Common.Repositories;
using MediatR;

namespace Application.Commands.Messaging
{
    public class UnhideConversation
    {
        public record UnhideConversationCommand(Guid UserId, Guid ConversationId) : IRequest<Result<string>>;

        public class UnhideConversationHandler(
            IConversationParticipantRepository participantRepository,
            IUnitOfWork unitOfWork) : IRequestHandler<UnhideConversationCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(UnhideConversationCommand request, CancellationToken cancellationToken)
            {
                var participant = await participantRepository.GetByConversationAndUserAsync(request.ConversationId, request.UserId);

                if (participant is null)
                {
                    return Result<string>.Failure("You're not a participant of this conversation");
                }

                participant.IsHidden = false;
                participantRepository.Update(participant);
                await unitOfWork.SaveAsync();

                return Result<string>.Success(string.Empty, "Conversation unhidden");
            }
        }
    }
}