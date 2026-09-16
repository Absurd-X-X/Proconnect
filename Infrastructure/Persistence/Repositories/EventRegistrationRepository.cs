using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class EventRegistrationRepository(ProConnectDbContext context) : IEventRegistrationRepository
    {
        public async Task AddAsync(EventRegistration registration)
        {
            await context.EventRegistrations.AddAsync(registration);
        }

        public async Task<EventRegistration?> GetByIdAsync(Guid id)
        {
            return await context.EventRegistrations
                .Include(r => r.Event)
                .Include(r => r.ProfessionalProfile)
                    .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<bool> ExistsAsync(Guid eventId, Guid professionalProfileId)
        {
            return await context.EventRegistrations.AnyAsync(r =>
                r.EventId == eventId &&
                r.ProfessionalProfileId == professionalProfileId &&
                r.Status == EventRegistrationStatus.Registered);
        }

        public async Task<EventRegistration?> GetByEventAndProfileAsync(Guid eventId, Guid professionalProfileId)
        {
            return await context.EventRegistrations
                .FirstOrDefaultAsync(r =>
                    r.EventId == eventId &&
                    r.ProfessionalProfileId == professionalProfileId &&
                    r.Status == EventRegistrationStatus.Registered);
        }

        public async Task<PageResponse<EventRegistration>> GetByEventIdAsync(
            PageRequest request,
            bool usePaging,
            Guid eventId,
            EventRegistrationStatus? status)
        {
            var query = context.EventRegistrations
                .Include(r => r.ProfessionalProfile)
                    .ThenInclude(p => p.User)
                .Where(r => r.EventId == eventId)
                .AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(r => r.Status == status.Value);
            }

            var ordered = query.OrderByDescending(r => r.RegisteredAt).AsQueryable();

            var totalCount = await ordered.CountAsync();

            if (!usePaging)
            {
                var allItems = await ordered.ToListAsync();

                return new PageResponse<EventRegistration>
                {
                    Items = allItems,
                    TotalCount = totalCount,
                    PageNumber = 1,
                    PageSize = totalCount
                };
            }

            var items = await ordered
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return new PageResponse<EventRegistration>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<PageResponse<EventRegistration>> GetByProfessionalProfileIdAsync(
            PageRequest request,
            bool usePaging,
            Guid professionalProfileId,
            EventRegistrationStatus? status,
            DateTime? eventStartDateFrom,
            DateTime? eventStartDateTo)
        {
            var query = context.EventRegistrations
                .Include(r => r.Event)
                    .ThenInclude(e => e.Company)
                .Where(r => r.ProfessionalProfileId == professionalProfileId)
                .AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(r => r.Status == status.Value);
            }

            if (eventStartDateFrom.HasValue)
            {
                query = query.Where(r => r.Event.StartDateTime >= eventStartDateFrom.Value);
            }

            if (eventStartDateTo.HasValue)
            {
                query = query.Where(r => r.Event.StartDateTime < eventStartDateTo.Value);
            }

            // Upcoming reads soonest-first; Past reads most-recent-first; the
            // unfiltered "Registered" tab reads most-recently-registered-first.
            var ordered = eventStartDateFrom.HasValue
                ? query.OrderBy(r => r.Event.StartDateTime)
                : eventStartDateTo.HasValue
                    ? query.OrderByDescending(r => r.Event.StartDateTime)
                    : query.OrderByDescending(r => r.RegisteredAt);

            var totalCount = await ordered.CountAsync();

            if (!usePaging)
            {
                var allItems = await ordered.ToListAsync();

                return new PageResponse<EventRegistration>
                {
                    Items = allItems,
                    TotalCount = totalCount,
                    PageNumber = 1,
                    PageSize = totalCount
                };
            }

            var items = await ordered
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return new PageResponse<EventRegistration>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public void Update(EventRegistration registration)
        {
            context.EventRegistrations.Update(registration);
        }
    }
}