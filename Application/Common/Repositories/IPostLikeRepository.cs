using Domain.Entities;
using Domain.Enums;

namespace Application.Common.Repositories
{
    public interface IPostLikeRepository
    {
        Task AddAsync(PostLike postLike);

        Task<PostLike?> GetByPostAndUserAsync(Guid postId, Guid userId);

        Task<int> GetTotalCountAsync(Guid postId);

        Task<Dictionary<ReactionType, int>> GetCountsByReactionTypeAsync(Guid postId);

        void Update(PostLike postLike);
    }
}