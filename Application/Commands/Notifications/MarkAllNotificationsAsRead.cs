using Application.Common.Dtos;
using Application.Services.Interfaces;
using MediatR;

namespace Application.Commands.Notifications
{
    public class MarkAllNotificationsAsRead
    {
        public record MarkAllNotificationsAsReadCommand(Guid UserId) : IRequest<Result<string>>;

        public class MarkAllNotificationsAsReadHandler(
            INotificationService notificationService)
            : IRequestHandler<MarkAllNotificationsAsReadCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(MarkAllNotificationsAsReadCommand request, CancellationToken cancellationToken)
            {
                await notificationService.MarkAllAsReadAsync(request.UserId);
                return Result<string>.Success(string.Empty, "All notifications marked as read");
            }
        }
    }
}