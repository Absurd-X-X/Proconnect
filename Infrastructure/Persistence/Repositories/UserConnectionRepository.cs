using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class UserConnectionRepository(ProConnectDbContext context) : IUserConnectionRepository
    {
        public async Task AddAsync(UserConnection connection)
        {
            await context.UserConnections.AddAsync(connection);
        }

        public async Task<UserConnection?> GetByIdAsync(Guid id)
        {
            return await context.UserConnections
                .Include(c => c.Sender)
                .Include(c => c.Reciever)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }

        public async Task<UserConnection?> GetConnectionBetweenUsersAsync(Guid userIdA, Guid userIdB)
        {
            return await context.UserConnections
                .FirstOrDefaultAsync(c =>
                    !c.IsDeleted &&
                    ((c.SenderId == userIdA && c.RecieverId == userIdB) ||
                     (c.SenderId == userIdB && c.RecieverId == userIdA)));
        }

        public async Task<PageResponse<UserConnection>> GetReceivedRequestsAsync(
            Guid userId,
            ConnectionStatus status,
            PageRequest request,
            bool usePaging)
        {
            var query = context.UserConnections
                .AsNoTracking()
                .Include(c => c.Sender)
                .Where(c => c.RecieverId == userId && c.ConnectionStatus == status && !c.IsDeleted)
                .OrderByDescending(c => c.DateCreated)
                .AsQueryable();

            if (!usePaging)
            {
                var allItems = await query.ToListAsync();

                return new PageResponse<UserConnection>
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

            return new PageResponse<UserConnection>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<PageResponse<UserConnection>> GetSentRequestsAsync(
            Guid userId,
            ConnectionStatus status,
            PageRequest request,
            bool usePaging)
        {
            var query = context.UserConnections
                .AsNoTracking()
                .Include(c => c.Reciever)
                .Where(c => c.SenderId == userId && c.ConnectionStatus == status && !c.IsDeleted)
                .OrderByDescending(c => c.DateCreated)
                .AsQueryable();

            if (!usePaging)
            {
                var allItems = await query.ToListAsync();

                return new PageResponse<UserConnection>
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

            return new PageResponse<UserConnection>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<PageResponse<UserConnection>> GetUserConnectionsAsync(
            Guid userId,
            PageRequest request,
            bool usePaging)
        {
            var query = context.UserConnections
                .AsNoTracking()
                .Include(c => c.Sender)
                .Include(c => c.Reciever)
                .Where(c => (c.SenderId == userId || c.RecieverId == userId)
                    && c.ConnectionStatus == ConnectionStatus.Accepted
                    && !c.IsDeleted)
                .OrderByDescending(c => c.DateUpdated)
                .AsQueryable();

            if (!usePaging)
            {
                var allItems = await query.ToListAsync();

                return new PageResponse<UserConnection>
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

            return new PageResponse<UserConnection>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<int> GetConnectionCountAsync(Guid userId)
        {
            return await context.UserConnections
                .Where(c => (c.SenderId == userId || c.RecieverId == userId)
                    && c.ConnectionStatus == ConnectionStatus.Accepted
                    && !c.IsDeleted)
                .CountAsync();
        }

        public void Update(UserConnection connection)
        {
            context.UserConnections.Update(connection);
        }

        public async Task<HashSet<Guid>> GetRelatedUserIdsAsync(Guid userId)
        {
            var ids = await context.UserConnections
                .Where(c => !c.IsDeleted && (c.SenderId == userId || c.RecieverId == userId))
                .Select(c => c.SenderId == userId ? c.RecieverId : c.SenderId)
                .ToListAsync();

            return ids.ToHashSet();
        }

        public async Task<int> GetMutualConnectionsCountAsync(Guid userIdA, Guid userIdB)
        {
            var aConnections = await context.UserConnections
                .Where(c => !c.IsDeleted && c.ConnectionStatus == ConnectionStatus.Accepted &&
                    (c.SenderId == userIdA || c.RecieverId == userIdA))
                .Select(c => c.SenderId == userIdA ? c.RecieverId : c.SenderId)
                .ToListAsync();

            var bConnections = await context.UserConnections
                .Where(c => !c.IsDeleted && c.ConnectionStatus == ConnectionStatus.Accepted &&
                    (c.SenderId == userIdB || c.RecieverId == userIdB))
                .Select(c => c.SenderId == userIdB ? c.RecieverId : c.SenderId)
                .ToListAsync();

            return aConnections.Intersect(bConnections).Count();
        }
    }
}