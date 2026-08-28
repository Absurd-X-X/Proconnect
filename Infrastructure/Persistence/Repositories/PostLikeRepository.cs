using Application.Common.Repositories;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class PostLikeRepository(ProConnectDbContext context) : IPostLikeRepository
    {
        public async Task AddAsync(PostLike postLike)
        {
            await context.PostLikes.AddAsync(postLike);
        }

        public async Task<PostLike?> GetByPostAndUserAsync(Guid postId, Guid userId)
        {
            return await context.PostLikes
                .FirstOrDefaultAsync(pl => pl.PostId == postId && pl.UserId == userId);
        }

        public async Task<int> GetTotalCountAsync(Guid postId)
        {
            return await context.PostLikes
                .Where(pl => pl.PostId == postId && !pl.IsDeleted)
                .CountAsync();
        }

        public async Task<Dictionary<ReactionType, int>> GetCountsByReactionTypeAsync(Guid postId)
        {
            return await context.PostLikes
                .Where(pl => pl.PostId == postId && !pl.IsDeleted)
                .GroupBy(pl => pl.ReactionType)
                .Select(g => new { g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Key, x => x.Count);
        }

        public void Update(PostLike postLike)
        {
            context.PostLikes.Update(postLike);
        }
    }
}