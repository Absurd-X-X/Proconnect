using Application.Common.Pagenation;
using Domain.Entities;

namespace Application.Common.Repositories
{
    public interface IConversationRepository
    {
        Task AddAsync(Conversation conversation);

        Task<Conversation?> GetByIdAsync(Guid id);

        Task<Conversation?> GetOneToOneConversationBetweenUsersAsync(Guid userIdA, Guid userIdB);

        Task<PageResponse<Conversation>> GetUserConversationsAsync(Guid userId, PageRequest request, bool usePaging);

        void Update(Conversation conversation);
    }
}