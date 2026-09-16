using Application.Common.Pagenation;
using Domain.Entities;
using Domain.Enums;

namespace Application.Common.Repositories
{
    public interface INotificationRepository
    {
        Task AddAsync(Notification notification);

        Task<Notification?> GetByIdAsync(Guid id, Guid userId);

        Task<List<Notification>> GetByIdsAsync(List<Guid> ids, Guid userId);

        Task<PageResponse<Notification>> GetByUserIdAsync(
            Guid userId,
            NotificationStatus? statusFilter,
            PageRequest request,
            bool usePaging);

        Task<int> GetUnreadCountAsync(Guid userId);

        Task<List<Notification>> GetAllUnreadAsync(Guid userId);

        Task<List<Notification>> GetUnreadBySourceAsync(Guid userId, NotificationSourceEntityType sourceEntityType, Guid sourceEntityId);

        void Update(Notification notification);
    }
}