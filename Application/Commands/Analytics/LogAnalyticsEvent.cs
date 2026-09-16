using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Commands.Analytics
{
    public class LogAnalyticsEvent
    {
        public record LogAnalyticsEventCommand(
            AnalyticsEventType EventType,
            AnalyticsSubjectType SubjectType,
            Guid SubjectId,
            Guid? ActorUserId,
            Guid? SubjectOwnerUserId,
            ReferrerSource? ReferrerSource
        ) : IRequest<Result<string>>;

        public class LogAnalyticsEventHandler(
            IAnalyticsEventRepository analyticsEventRepository,
            IUnitOfWork unitOfWork)
            : IRequestHandler<LogAnalyticsEventCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(LogAnalyticsEventCommand request, CancellationToken cancellationToken)
            {
                if (request.ActorUserId.HasValue && request.ActorUserId == request.SubjectOwnerUserId)
                    return Result<string>.Success("skipped", "Self-view not logged");

                if (request.ActorUserId.HasValue &&
                    (request.EventType == AnalyticsEventType.ProfileView || request.EventType == AnalyticsEventType.JobView))
                {
                    var since = DateTime.UtcNow.AddHours(-24);
                    var alreadyLogged = await analyticsEventRepository.ExistsRecentAsync(
                        request.EventType, request.SubjectId, request.ActorUserId.Value, since);

                    if (alreadyLogged)
                        return Result<string>.Success("skipped", "Duplicate view within 24 hours not logged");
                }

                var evt = new AnalyticsEvent
                {
                    EventType = request.EventType,
                    SubjectType = request.SubjectType,
                    SubjectId = request.SubjectId,
                    ActorUserId = request.ActorUserId,
                    ReferrerSource = request.ReferrerSource
                };

                await analyticsEventRepository.LogAsync(evt);
                await unitOfWork.SaveAsync();

                return Result<string>.Success(evt.Id.ToString(), "Event logged");
            }
        }
    }
}