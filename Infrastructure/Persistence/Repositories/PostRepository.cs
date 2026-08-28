using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class PostRepository(ProConnectDbContext context) : IPostRepository
    {
        public async Task AddAsync(Post post)
        {
            await context.Posts.AddAsync(post);
        }

        public async Task<Post?> GetByIdAsync(Guid id)
        {
            return await context.Posts
                .Include(p => p.User)
                .Include(p => p.Attachments.Where(a => !a.IsDeleted))
                .Include(p => p.OriginalPost)
                    .ThenInclude(op => op!.User)
                .Include(p => p.OriginalPost)
                    .ThenInclude(op => op!.Attachments.Where(a => !a.IsDeleted))
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        }

        public async Task<PageResponse<Post>> GetFeedByAuthorIdsAsync(
            IEnumerable<Guid> authorIds,
            PageRequest request,
            bool usePaging)
        {
            var query = context.Posts
                .AsNoTracking()
                .Include(p => p.User)
                .Include(p => p.Attachments.Where(a => !a.IsDeleted))
                .Include(p => p.OriginalPost)
                    .ThenInclude(op => op!.User)
                .Include(p => p.OriginalPost)
                    .ThenInclude(op => op!.Attachments.Where(a => !a.IsDeleted))
                .Where(p => authorIds.Contains(p.UserId) && !p.IsDeleted)
                .OrderByDescending(p => p.DateCreated)
                .AsQueryable();

            return await PaginateAsync(query, request, usePaging);
        }

        public async Task<PageResponse<Post>> GetAllPublicAsync(PageRequest request, bool usePaging)
        {
            var query = context.Posts
                .AsNoTracking()
                .Include(p => p.User)
                .Include(p => p.Attachments.Where(a => !a.IsDeleted))
                .Include(p => p.OriginalPost)
                    .ThenInclude(op => op!.User)
                .Include(p => p.OriginalPost)
                    .ThenInclude(op => op!.Attachments.Where(a => !a.IsDeleted))
                .Where(p => p.Visibility == Visibility.Public && !p.IsDeleted)
                .OrderByDescending(p => p.DateCreated)
                .AsQueryable();

            return await PaginateAsync(query, request, usePaging);
        }

        public async Task<PageResponse<Post>> GetByUserIdAsync(Guid userId, PageRequest request, bool usePaging)
        {
            var query = context.Posts
                .AsNoTracking()
                .Include(p => p.User)
                .Include(p => p.Attachments.Where(a => !a.IsDeleted))
                .Include(p => p.OriginalPost)
                    .ThenInclude(op => op!.User)
                .Include(p => p.OriginalPost)
                    .ThenInclude(op => op!.Attachments.Where(a => !a.IsDeleted))
                .Where(p => p.UserId == userId && !p.IsDeleted)
                .OrderByDescending(p => p.DateCreated)
                .AsQueryable();

            return await PaginateAsync(query, request, usePaging);
        }

        public async Task<int> GetCommentsCountAsync(Guid postId)
        {
            return await context.Comments
                .Where(c => c.PostId == postId)
                .CountAsync();
        }

        public async Task<int> GetSharesCountAsync(Guid postId)
        {
            return await context.Posts
                .Where(p => p.OriginalPostId == postId && !p.IsDeleted)
                .CountAsync();
        }

        public void Update(Post post)
        {
            context.Posts.Update(post);
        }

        private static async Task<PageResponse<Post>> PaginateAsync(
            IQueryable<Post> query,
            PageRequest request,
            bool usePaging)
        {
            if (!usePaging)
            {
                var allItems = await query.ToListAsync();

                return new PageResponse<Post>
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

            return new PageResponse<Post>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}