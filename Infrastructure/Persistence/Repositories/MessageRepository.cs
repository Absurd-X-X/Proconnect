using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class MessageRepository(ProConnectDbContext context) : IMessageRepository
    {
        public async Task AddAsync(Message message)
        {
            await context.Messages.AddAsync(message);
        }

        public async Task<Message?> GetByIdAsync(Guid id)
        {
            return await context.Messages
                .Include(m => m.User)
                .Include(m => m.Conversation)
                .FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted);
        }

        public async Task<PageResponse<Message>> GetByConversationIdAsync(
            Guid conversationId,
            PageRequest request,
            bool usePaging)
        {
            // Newest first — matches "load older messages on scroll-up" pagination;
            // the frontend reverses the page's items for chronological display.
            var query = context.Messages
                .AsNoTracking()
                .Include(m => m.User)
                .Where(m => m.ConversationId == conversationId && !m.IsDeleted)
                .OrderByDescending(m => m.DateCreated)
                .AsQueryable();

            if (!usePaging)
            {
                var allItems = await query.ToListAsync();

                return new PageResponse<Message>
                {
                    Items = allItems,
                    TotalCount = allItems.Count,
                    PageNumber = 1,
                    PageSize = allItems.Count
                };
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return new PageResponse<Message>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<int> GetUnreadCountAsync(Guid conversationId, Guid userId, DateTime? lastReadAt)
        {
            var query = context.Messages
                .Where(m => m.ConversationId == conversationId && !m.IsDeleted && m.UserId != userId);

            if (lastReadAt.HasValue)
            {
                query = query.Where(m => m.DateCreated > lastReadAt.Value);
            }

            return await query.CountAsync();
        }

        public void Update(Message message)
        {
            context.Messages.Update(message);
        }
    }
}