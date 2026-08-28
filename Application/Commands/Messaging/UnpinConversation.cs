using Application.Common.Dtos;
using Application.Common.Repositories;
using MediatR;

namespace Application.Commands.Messaging
{
    public class UnpinConversation
    {
        public record UnpinConversationCommand(Guid UserId, Guid ConversationId) : IRequest<Result<string>>;

        public class UnpinConversationHandler(
            IConversationParticipantRepository participantRepository,
            IUnitOfWork unitOfWork) : IRequestHandler<UnpinConversationCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(UnpinConversationCommand request, CancellationToken cancellationToken)
            {
                var participant = await participantRepository.GetByConversationAndUserAsync(request.ConversationId, request.UserId);

                if (participant is null)
                {
                    return Result<string>.Failure("You're not a participant of this conversation");
                }

                participant.IsPinned = false;
                participantRepository.Update(participant);
                await unitOfWork.SaveAsync();

                return Result<string>.Success(string.Empty, "Conversation unpinned");
            }
        }
    }
}