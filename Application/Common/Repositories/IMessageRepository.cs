using Application.Common.Pagenation;
using Domain.Entities;

namespace Application.Common.Repositories
{
    public interface IMessageRepository
    {
        Task AddAsync(Message message);

        Task<Message?> GetByIdAsync(Guid id);

        Task<PageResponse<Message>> GetByConversationIdAsync(Guid conversationId, PageRequest request, bool usePaging);

        Task<int> GetUnreadCountAsync(Guid conversationId, Guid userId, DateTime? lastReadAt);

        void Update(Message message);
    }
}