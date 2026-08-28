using Application.Common.Pagenation;
using Domain.Entities;

namespace Application.Common.Repositories
{
    public interface IPostRepository
    {
        Task AddAsync(Post post);

        Task<Post?> GetByIdAsync(Guid id);

        Task<PageResponse<Post>> GetFeedByAuthorIdsAsync(
            IEnumerable<Guid> authorIds,
            PageRequest request,
            bool usePaging);

        Task<PageResponse<Post>> GetAllPublicAsync(PageRequest request, bool usePaging);

        Task<PageResponse<Post>> GetByUserIdAsync(Guid userId, PageRequest request, bool usePaging);

        Task<int> GetCommentsCountAsync(Guid postId);

        Task<int> GetSharesCountAsync(Guid postId);

        void Update(Post post);
    }
}