using Application.Common.Dtos;
using Application.Common.Repositories;
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

                return Result<string>.Success(string.Empty, "Added to the group");
            }
        }
    }
}