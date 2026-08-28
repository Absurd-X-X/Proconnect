using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class CommentRepository(ProConnectDbContext context) : ICommentRepository
    {
        public async Task AddAsync(Comment comment)
        {
            await context.Comments.AddAsync(comment);
        }

        public async Task<Comment?> GetByIdAsync(Guid id)
        {
            return await context.Comments
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<PageResponse<Comment>> GetByPostIdAsync(Guid postId, PageRequest request, bool usePaging)
        {
            var query = context.Comments
                .AsNoTracking()
                .Include(c => c.User)
                .Where(c => c.PostId == postId)
                .OrderByDescending(c => c.DateCreated)
                .AsQueryable();

            if (!usePaging)
            {
                var allItems = await query.ToListAsync();

                return new PageResponse<Comment>
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

            return new PageResponse<Comment>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public void Update(Comment comment)
        {
            context.Comments.Update(comment);
        }

        public void Delete(Comment comment)
        {
            context.Comments.Remove(comment);
        }
    }
}