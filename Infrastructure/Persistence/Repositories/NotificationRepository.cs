using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class NotificationRepository(ProConnectDbContext context) : INotificationRepository
    {
        public async Task AddAsync(Notification notification)
        {
            await context.Notifications.AddAsync(notification);
        }

        public async Task<Notification?> GetByIdAsync(Guid id, Guid userId)
        {
            return await context.Notifications
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId && !n.IsDeleted);
        }

        public async Task<List<Notification>> GetByIdsAsync(List<Guid> ids, Guid userId)
        {
            return await context.Notifications
                .Where(n => ids.Contains(n.Id) && n.UserId == userId && !n.IsDeleted)
                .ToListAsync();
        }

        public async Task<PageResponse<Notification>> GetByUserIdAsync(
            Guid userId,
            NotificationStatus? statusFilter,
            PageRequest request,
            bool usePaging)
        {
            var query = context.Notifications
                .AsNoTracking()
                .Where(n => n.UserId == userId && !n.IsDeleted)
                .AsQueryable();

            if (statusFilter.HasValue)
            {
                query = query.Where(n => n.Status == statusFilter.Value);
            }

            query = query.OrderByDescending(n => n.DateCreated);

            if (!usePaging)
            {
                var allItems = await query.ToListAsync();

                return new PageResponse<Notification>
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

            return new PageResponse<Notification>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<int> GetUnreadCountAsync(Guid userId)
        {
            return await context.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsDeleted && n.Status == NotificationStatus.Unread);
        }

        public async Task<List<Notification>> GetAllUnreadAsync(Guid userId)
        {
            return await context.Notifications
                .Where(n => n.UserId == userId && !n.IsDeleted && n.Status == NotificationStatus.Unread)
                .ToListAsync();
        }

        public async Task<List<Notification>> GetUnreadBySourceAsync(Guid userId, NotificationSourceEntityType sourceEntityType, Guid sourceEntityId)
        {
            return await context.Notifications
                .Where(n => n.UserId == userId
                    && !n.IsDeleted
                    && n.Status == NotificationStatus.Unread
                    && n.SourceEntityType == sourceEntityType
                    && n.SourceEntityId == sourceEntityId)
                .ToListAsync();
        }

        public void Update(Notification notification)
        {
            context.Notifications.Update(notification);
        }
    }
}