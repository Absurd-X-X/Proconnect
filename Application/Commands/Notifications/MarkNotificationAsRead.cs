using Application.Common.Dtos;
using Application.Services.Interfaces;
using MediatR;

namespace Application.Commands.Notifications
{
    public class MarkNotificationAsRead
    {
        public record MarkNotificationAsReadCommand(Guid UserId, Guid NotificationId) : IRequest<Result<string>>;

        public class MarkNotificationAsReadHandler(
            INotificationService notificationService)
            : IRequestHandler<MarkNotificationAsReadCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
            {
                await notificationService.MarkAsReadAsync(request.NotificationId, request.UserId);
                return Result<string>.Success(string.Empty, "Notification marked as read");
            }
        }
    }
}