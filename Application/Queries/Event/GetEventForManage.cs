using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.Queries.Event
{
    public class GetEventForManage
    {
        public record GetEventForManageQuery(Guid UserId, Guid EventId) : IRequest<Result<GetEventForManageResponse>>;

        public class GetEventForManageHandler(
            IRecruiterProfileRepository recruiterProfileRepository,
            IEventRepository eventRepository)
            : IRequestHandler<GetEventForManageQuery, Result<GetEventForManageResponse>>
        {
            public async Task<Result<GetEventForManageResponse>> Handle(
                GetEventForManageQuery request,
                CancellationToken cancellationToken)
            {
                var recruiterProfile = await recruiterProfileRepository.GetByUserIdAsync(request.UserId);

                if (recruiterProfile is null || recruiterProfile.CompanyId is null)
                    return Result<GetEventForManageResponse>.Failure("Recruiter profile not found");

                var @event = await eventRepository.GetWithDetailsAsync(request.EventId);

                if (@event is null)
                    return Result<GetEventForManageResponse>.Failure("Event not found");

                if (@event.CompanyId != recruiterProfile.CompanyId)
                    return Result<GetEventForManageResponse>.Failure("You are not authorized to manage this event");

                var registeredCount = @event.Registrations.Count(r => r.Status != EventRegistrationStatus.CancelledByUser);
                var attendedCount = @event.Registrations.Count(r => r.Status == EventRegistrationStatus.Attended);

                var response = new GetEventForManageResponse(
                    @event.Id,
                    @event.Title,
                    @event.Description,
                    @event.CoverImageUrl,
                    @event.EventType,
                    @event.Category,
                    @event.StartDateTime,
                    @event.EndDateTime,
                    @event.LocationType,
                    @event.Location,
                    @event.OnlineMeetingLink,
                    @event.Visibility,
                    @event.RegistrationType,
                    @event.AttendeeLimit,
                    @event.Status,
                    @event.CancellationReason,
                    registeredCount,
                    attendedCount,
                    @event.Company.Name,
                    @event.Speakers
                        .OrderBy(s => s.DisplayOrder)
                        .Select(s => new EventSpeakerResponse(s.Id, s.Name, s.Title, s.Company, s.PhotoUrl, s.LinkedInUrl))
                        .ToList(),
                    @event.AgendaItems
                        .OrderBy(a => a.DisplayOrder)
                        .Select(a => new EventAgendaItemResponse(a.Id, a.Title, a.Description, a.StartTime, a.EndTime))
                        .ToList());

                return Result<GetEventForManageResponse>.Success(response, "Event retrieved successfully");
            }
        }
    }

    public record EventSpeakerResponse(
        Guid Id, string Name, string? Title, string? Company, string? PhotoUrl, string? LinkedInUrl);

    public record EventAgendaItemResponse(
        Guid Id, string Title, string? Description, DateTime StartTime, DateTime EndTime);

    public record GetEventForManageResponse(
        Guid Id,
        string Title,
        string Description,
        string? CoverImageUrl,
        EventType EventType,
        string? Category,
        DateTime StartDateTime,
        DateTime EndDateTime,
        EventLocationType LocationType,
        string? Location,
        string? OnlineMeetingLink,
        EventVisibility Visibility,
        EventRegistrationType RegistrationType,
        int? AttendeeLimit,
        EventStatus Status,
        string? CancellationReason,
        int RegisteredCount,
        int AttendedCount,
        string CompanyName,
        List<EventSpeakerResponse> Speakers,
        List<EventAgendaItemResponse> AgendaItems);
}