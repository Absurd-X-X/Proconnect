using Application.Common.Pagenation;
using Domain.Entities;

namespace Application.Common.Repositories
{
    public interface IUserFollowRepository
    {
        Task AddAsync(UserFollow follow);

        Task<UserFollow?> GetByFollowerAndFollowingAsync(Guid followerId, Guid followingId);

        Task<PageResponse<UserFollow>> GetFollowersAsync(Guid userId, PageRequest request, bool usePaging);

        Task<PageResponse<UserFollow>> GetFollowingAsync(Guid userId, PageRequest request, bool usePaging);

        Task<int> GetFollowerCountAsync(Guid userId);

        Task<int> GetFollowingCountAsync(Guid userId);

        void Delete(UserFollow follow);
    }
}