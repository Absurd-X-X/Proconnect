using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class UserFollowRepository(ProConnectDbContext context) : IUserFollowRepository
    {
        public async Task AddAsync(UserFollow follow)
        {
            await context.UserFollows.AddAsync(follow);
        }

        public async Task<UserFollow?> GetByFollowerAndFollowingAsync(Guid followerId, Guid followingId)
        {
            return await context.UserFollows
                .FirstOrDefaultAsync(f =>
                    f.FollowerId == followerId &&
                    f.FollowingId == followingId &&
                    !f.IsDeleted);
        }

        public async Task<PageResponse<UserFollow>> GetFollowersAsync(Guid userId, PageRequest request, bool usePaging)
        {
            var query = context.UserFollows
                .AsNoTracking()
                .Include(f => f.Follower)
                .Where(f => f.FollowingId == userId && !f.IsDeleted)
                .OrderByDescending(f => f.DateCreated)
                .AsQueryable();

            if (!usePaging)
            {
                var allItems = await query.ToListAsync();

                return new PageResponse<UserFollow>
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

            return new PageResponse<UserFollow>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<PageResponse<UserFollow>> GetFollowingAsync(Guid userId, PageRequest request, bool usePaging)
        {
            var query = context.UserFollows
                .AsNoTracking()
                .Include(f => f.Following)
                .Where(f => f.FollowerId == userId && !f.IsDeleted)
                .OrderByDescending(f => f.DateCreated)
                .AsQueryable();

            if (!usePaging)
            {
                var allItems = await query.ToListAsync();

                return new PageResponse<UserFollow>
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

            return new PageResponse<UserFollow>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<int> GetFollowerCountAsync(Guid userId)
        {
            return await context.UserFollows
                .Where(f => f.FollowingId == userId && !f.IsDeleted)
                .CountAsync();
        }

        public async Task<int> GetFollowingCountAsync(Guid userId)
        {
            return await context.UserFollows
                .Where(f => f.FollowerId == userId && !f.IsDeleted)
                .CountAsync();
        }

        public void Delete(UserFollow follow)
        {
            context.UserFollows.Remove(follow);
        }
    }
}