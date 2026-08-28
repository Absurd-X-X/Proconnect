using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class ConversationParticipantRepository(ProConnectDbContext context) : IConversationParticipantRepository
    {
        public async Task AddAsync(ConversationParticipant participant)
        {
            await context.ConversationParticipants.AddAsync(participant);
        }

        public async Task<ConversationParticipant?> GetByConversationAndUserAsync(Guid conversationId, Guid userId)
        {
            return await context.ConversationParticipants
                .FirstOrDefaultAsync(p => p.ConversationId == conversationId && p.UserId == userId && !p.IsDeleted);
        }

        public async Task<List<ConversationParticipant>> GetByConversationIdAsync(Guid conversationId)
        {
            return await context.ConversationParticipants
                .AsNoTracking()
                .Include(p => p.User)
                .Where(p => p.ConversationId == conversationId && !p.IsDeleted)
                .ToListAsync();
        }

        public async Task<PageResponse<Conversation>> GetUserConversationsAsync(
        Guid userId,
        PageRequest request,
        bool usePaging)
        {
            var allMatching = await context.Conversations
                .AsNoTracking()
                .Include(c => c.Participants.Where(p => !p.IsDeleted))
                    .ThenInclude(p => p.User)
                .Where(c => !c.IsDeleted && c.Participants.Any(p => !p.IsDeleted && p.UserId == userId && !p.IsHidden))
                .ToListAsync();

            var ordered = allMatching
                .OrderByDescending(c => c.Participants.First(p => p.UserId == userId).IsPinned)
                .ThenByDescending(c => c.DateModified)
                .ToList();

            if (!usePaging)
            {
                return new PageResponse<Conversation>
                {
                    Items = ordered,
                    TotalCount = ordered.Count,
                    PageNumber = 1,
                    PageSize = ordered.Count
                };
            }

            var items = ordered
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            return new PageResponse<Conversation>
            {
                Items = items,
                TotalCount = ordered.Count,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public void Update(ConversationParticipant participant)
        {
            context.ConversationParticipants.Update(participant);
        }
    }
}