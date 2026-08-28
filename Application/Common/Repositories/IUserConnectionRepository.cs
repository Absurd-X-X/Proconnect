using Application.Common.Pagenation;
using Domain.Entities;
using Domain.Enums;

namespace Application.Common.Repositories
{
    public interface IUserConnectionRepository
    {
        Task AddAsync(UserConnection connection);

        Task<UserConnection?> GetByIdAsync(Guid id);

        Task<UserConnection?> GetConnectionBetweenUsersAsync(Guid userIdA, Guid userIdB);

        Task<PageResponse<UserConnection>> GetReceivedRequestsAsync(
            Guid userId,
            ConnectionStatus status,
            PageRequest request,
            bool usePaging);

        Task<PageResponse<UserConnection>> GetSentRequestsAsync(
            Guid userId,
            ConnectionStatus status,
            PageRequest request,
            bool usePaging);

        Task<PageResponse<UserConnection>> GetUserConnectionsAsync(
            Guid userId,
            PageRequest request,
            bool usePaging);

        Task<int> GetConnectionCountAsync(Guid userId);

        void Update(UserConnection connection);

        Task<HashSet<Guid>> GetRelatedUserIdsAsync(Guid userId);

        Task<int> GetMutualConnectionsCountAsync(Guid userIdA, Guid userIdB);
    }
}