using Application.Common.Pagenation;
using Domain.Entities;

namespace Application.Common.Repositories
{
    public interface IConversationParticipantRepository
    {
        Task AddAsync(ConversationParticipant participant);

        Task<ConversationParticipant?> GetByConversationAndUserAsync(Guid conversationId, Guid userId);

        Task<List<ConversationParticipant>> GetByConversationIdAsync(Guid conversationId);

        Task<PageResponse<Conversation>> GetUserConversationsAsync(
        Guid userId,
        PageRequest request,
        bool usePaging);

        void Update(ConversationParticipant participant);
    }
}