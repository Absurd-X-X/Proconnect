using Application.Common.Dtos;
using Application.Common.Repositories;
using MediatR;

namespace Application.Queries.Messaging
{
    public class GetConversationParticipants
    {
        public record GetConversationParticipantsQuery(Guid UserId, Guid ConversationId) : IRequest<Result<List<ParticipantResponse>>>;

        public class GetConversationParticipantsHandler(
            IConversationParticipantRepository participantRepository)
            : IRequestHandler<GetConversationParticipantsQuery, Result<List<ParticipantResponse>>>
        {
            public async Task<Result<List<ParticipantResponse>>> Handle(GetConversationParticipantsQuery request, CancellationToken cancellationToken)
            {
                var requester = await participantRepository.GetByConversationAndUserAsync(request.ConversationId, request.UserId);

                if (requester is null)
                {
                    return Result<List<ParticipantResponse>>.Failure("You're not a participant of this conversation");
                }

                var participants = await participantRepository.GetByConversationIdAsync(request.ConversationId);

                var response = participants
                    .Select(p => new ParticipantResponse(p.UserId, p.User.FirstName, p.User.LastName, p.User.ProfilePictureUrl))
                    .ToList();

                return Result<List<ParticipantResponse>>.Success(response, "Participants retrieved successfully");
            }
        }
    }

    public record ParticipantResponse(Guid UserId, string FirstName, string LastName, string? ProfilePictureUrl);
}