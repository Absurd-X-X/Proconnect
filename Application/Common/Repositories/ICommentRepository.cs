using Application.Common.Pagenation;
using Domain.Entities;

namespace Application.Common.Repositories
{
    public interface ICommentRepository
    {
        Task AddAsync(Comment comment);

        Task<Comment?> GetByIdAsync(Guid id);

        Task<PageResponse<Comment>> GetByPostIdAsync(Guid postId, PageRequest request, bool usePaging);

        void Update(Comment comment);

        void Delete(Comment comment);
    }
}