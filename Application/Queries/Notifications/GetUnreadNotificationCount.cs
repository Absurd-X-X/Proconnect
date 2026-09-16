using Application.Common.Dtos;
using Application.Common.Repositories;
using MediatR;

namespace Application.Queries.Notifications
{
    public class GetUnreadNotificationCount
    {
        public record GetUnreadNotificationCountQuery(Guid UserId) : IRequest<Result<int>>;

        public class GetUnreadNotificationCountHandler(
            INotificationRepository notificationRepository)
            : IRequestHandler<GetUnreadNotificationCountQuery, Result<int>>
        {
            public async Task<Result<int>> Handle(GetUnreadNotificationCountQuery request, CancellationToken cancellationToken)
            {
                var count = await notificationRepository.GetUnreadCountAsync(request.UserId);
                return Result<int>.Success(count, "Unread count retrieved successfully");
            }
        }
    }
}