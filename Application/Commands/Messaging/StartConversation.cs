using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Commands.Messaging
{
    public class StartConversation
    {
        public record StartConversationCommand(Guid UserId, Guid RecipientId) : IRequest<Result<ConversationResponse>>;

        public class StartConversationHandler(
            IConversationRepository conversationRepository,
            IConversationParticipantRepository participantRepository,
            IUserConnectionRepository connectionRepository,
            IUnitOfWork unitOfWork) : IRequestHandler<StartConversationCommand, Result<ConversationResponse>>
        {
            public async Task<Result<ConversationResponse>> Handle(StartConversationCommand request, CancellationToken cancellationToken)
            {
                if (request.UserId == request.RecipientId)
                {
                    return Result<ConversationResponse>.Failure("You cannot start a conversation with yourself");
                }

                var connection = await connectionRepository.GetConnectionBetweenUsersAsync(request.UserId, request.RecipientId);

                if (connection is null || connection.ConnectionStatus != ConnectionStatus.Accepted)
                {
                    return Result<ConversationResponse>.Failure("You can only message people you're connected with");
                }

                var existing = await conversationRepository.GetOneToOneConversationBetweenUsersAsync(request.UserId, request.RecipientId);

                if (existing is not null)
                {
                    return Result<ConversationResponse>.Success(
                        new ConversationResponse(existing.Id, existing.IsGroup, existing.Title),
                        "Conversation already exists");
                }

                var conversation = new Conversation
                {
                    IsGroup = false,
                    Title = null,
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

                await participantRepository.AddAsync(new ConversationParticipant
                {
                    ConversationId = conversation.Id,
                    UserId = request.RecipientId,
                    CreatedBy = request.UserId.ToString()
                });

                await unitOfWork.SaveAsync();

                return Result<ConversationResponse>.Success(
                    new ConversationResponse(conversation.Id, conversation.IsGroup, conversation.Title),
                    "Conversation started");
            }
        }
    }

    public record ConversationResponse(Guid Id, bool IsGroup, string? Title);
}