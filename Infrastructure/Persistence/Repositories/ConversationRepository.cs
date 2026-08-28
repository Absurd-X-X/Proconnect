using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class ConversationRepository(ProConnectDbContext context) : IConversationRepository
    {
        public async Task AddAsync(Conversation conversation)
        {
            await context.Conversations.AddAsync(conversation);
        }

        public async Task<Conversation?> GetByIdAsync(Guid id)
        {
            return await context.Conversations
                .Include(c => c.Participants.Where(p => !p.IsDeleted))
                    .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }

        public async Task<Conversation?> GetOneToOneConversationBetweenUsersAsync(Guid userIdA, Guid userIdB)
        {
            return await context.Conversations
                .Include(c => c.Participants.Where(p => !p.IsDeleted))
                    .ThenInclude(p => p.User)
                .Where(c => !c.IsDeleted && !c.IsGroup)
                .Where(c => c.Participants.Count(p => !p.IsDeleted) == 2
                    && c.Participants.Any(p => !p.IsDeleted && p.UserId == userIdA)
                    && c.Participants.Any(p => !p.IsDeleted && p.UserId == userIdB))
                .FirstOrDefaultAsync();
        }

        public async Task<PageResponse<Conversation>> GetUserConversationsAsync(
            Guid userId,
            PageRequest request,
            bool usePaging)
        {
            var query = context.Conversations
                .AsNoTracking()
                .Include(c => c.Participants.Where(p => !p.IsDeleted))
                    .ThenInclude(p => p.User)
                .Where(c => !c.IsDeleted && c.Participants.Any(p => !p.IsDeleted && p.UserId == userId))
                .OrderByDescending(c => c.DateModified)
                .AsQueryable();

            if (!usePaging)
            {
                var allItems = await query.ToListAsync();

                return new PageResponse<Conversation>
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

            return new PageResponse<Conversation>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public void Update(Conversation conversation)
        {
            context.Conversations.Update(conversation);
        }
    }
}