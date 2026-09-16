using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class SavedEventRepository(ProConnectDbContext context) : ISavedEventRepository
    {
        public async Task AddAsync(SavedEvent savedEvent)
        {
            await context.SavedEvents.AddAsync(savedEvent);
        }

        public async Task<bool> ExistsAsync(Guid eventId, Guid professionalProfileId)
        {
            return await context.SavedEvents.AnyAsync(s =>
                s.EventId == eventId && s.ProfessionalProfileId == professionalProfileId);
        }

        public async Task<SavedEvent?> GetByEventAndProfileAsync(Guid eventId, Guid professionalProfileId)
        {
            return await context.SavedEvents
                .FirstOrDefaultAsync(s =>
                    s.EventId == eventId && s.ProfessionalProfileId == professionalProfileId);
        }

        public async Task<PageResponse<SavedEvent>> GetByProfessionalProfileIdAsync(
            PageRequest request,
            bool usePaging,
            Guid professionalProfileId)
        {
            var query = context.SavedEvents
                .Include(s => s.Event)
                    .ThenInclude(e => e.Company)
                .Where(s => s.ProfessionalProfileId == professionalProfileId)
                .OrderByDescending(s => s.DateCreated)
                .AsQueryable();

            var totalCount = await query.CountAsync();

            if (!usePaging)
            {
                var allItems = await query.ToListAsync();

                return new PageResponse<SavedEvent>
                {
                    Items = allItems,
                    TotalCount = totalCount,
                    PageNumber = 1,
                    PageSize = totalCount
                };
            }

            var items = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return new PageResponse<SavedEvent>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public void Delete(SavedEvent savedEvent)
        {
            context.SavedEvents.Remove(savedEvent);
        }
    }
}