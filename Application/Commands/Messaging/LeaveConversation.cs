using Application.Common.Dtos;
using Application.Common.Repositories;
using MediatR;

namespace Application.Commands.Messaging
{
    public class LeaveConversation
    {
        public record LeaveConversationCommand(Guid UserId, Guid ConversationId) : IRequest<Result<string>>;

        public class LeaveConversationHandler(
            IConversationParticipantRepository participantRepository,
            IUnitOfWork unitOfWork) : IRequestHandler<LeaveConversationCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(LeaveConversationCommand request, CancellationToken cancellationToken)
            {
                var participant = await participantRepository.GetByConversationAndUserAsync(request.ConversationId, request.UserId);

                if (participant is null)
                {
                    return Result<string>.Failure("You're not a participant of this conversation");
                }

                participant.IsDeleted = true;
                participantRepository.Update(participant);
                await unitOfWork.SaveAsync();

                return Result<string>.Success(string.Empty, "You left the conversation");
            }
        }
    }
}