using Application.Common.Dtos;
using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.Queries.Notifications
{
    public class GetNotifications
    {
        public record GetNotificationsQuery(Guid UserId, string? Status, PageRequest PageRequest, bool UsePaging)
            : IRequest<Result<PageResponse<NotificationResponse>>>;

        public class GetNotificationsHandler(
            INotificationRepository notificationRepository)
            : IRequestHandler<GetNotificationsQuery, Result<PageResponse<NotificationResponse>>>
        {
            public async Task<Result<PageResponse<NotificationResponse>>> Handle(
                GetNotificationsQuery request, CancellationToken cancellationToken)
            {
                NotificationStatus? statusFilter = null;

                if (!string.IsNullOrWhiteSpace(request.Status)
                    && Enum.TryParse<NotificationStatus>(request.Status, true, out var parsed))
                {
                    statusFilter = parsed;
                }

                var page = await notificationRepository.GetByUserIdAsync(
                    request.UserId, statusFilter, request.PageRequest, request.UsePaging);

                var items = page.Items.Select(n => new NotificationResponse(
                    n.Id,
                    n.ActorUserId,
                    n.ActorName,
                    n.ActorAvatarUrl,
                    n.Title,
                    n.Message,
                    n.Type.ToString(),
                    n.Status.ToString(),
                    n.SourceEntityType?.ToString(),
                    n.SourceEntityId,
                    n.ActionUrl,
                    n.DateCreated,
                    n.DateRead)).ToList();

                var response = new PageResponse<NotificationResponse>
                {
                    Items = items,
                    TotalCount = page.TotalCount,
                    PageNumber = page.PageNumber,
                    PageSize = page.PageSize
                };

                return Result<PageResponse<NotificationResponse>>.Success(response, "Notifications retrieved successfully");
            }
        }
    }

    public record NotificationResponse(
        Guid Id,
        Guid? ActorUserId,
        string? ActorName,
        string? ActorAvatarUrl,
        string Title,
        string Message,
        string Type,
        string Status,
        string? SourceEntityType,
        Guid? SourceEntityId,
        string? ActionUrl,
        DateTime DateCreated,
        DateTime? DateRead);
}