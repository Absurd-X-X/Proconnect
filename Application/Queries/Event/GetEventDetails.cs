using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.Queries.Event
{
    public class GetEventDetails
    {
        public record GetEventDetailsQuery(Guid UserId, Guid EventId) : IRequest<Result<GetEventDetailsResponse>>;

        public class GetEventDetailsHandler(
            IProfessionalProfileRepository professionalProfileRepository,
            IEventRepository eventRepository,
            IEventRegistrationRepository eventRegistrationRepository,
            ISavedEventRepository savedEventRepository)
            : IRequestHandler<GetEventDetailsQuery, Result<GetEventDetailsResponse>>
        {
            public async Task<Result<GetEventDetailsResponse>> Handle(
                GetEventDetailsQuery request,
                CancellationToken cancellationToken)
            {
                var @event = await eventRepository.GetWithDetailsAsync(request.EventId);

                if (@event is null)
                    return Result<GetEventDetailsResponse>.Failure("Event not found");

                var professionalProfile = await professionalProfileRepository.GetByUserIdAsync(request.UserId);

                var isRegistered = false;
                var isSaved = false;

                if (professionalProfile is not null)
                {
                    isRegistered = await eventRegistrationRepository.ExistsAsync(@event.Id, professionalProfile.Id);
                    isSaved = await savedEventRepository.ExistsAsync(@event.Id, professionalProfile.Id);
                }

                var registered = @event.Registrations.ToList();
                var goingCount = registered.Count;

                var attendeeAvatars = registered
                    .Take(6)
                    .Select(r => new AttendeeAvatarResponse(
                        r.ProfessionalProfile.Id,
                        r.FullName,
                        r.ProfessionalProfile.User.ProfilePictureUrl))
                    .ToList();

                var response = new GetEventDetailsResponse(
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
                    @event.AttendeeLimit,
                    @event.Status,
                    @event.Company.Id,
                    @event.Company.Name,
                    @event.Company.LogoUrl,
                    goingCount,
                    attendeeAvatars,
                    isRegistered,
                    isSaved,
                    @event.Speakers
                        .OrderBy(s => s.DisplayOrder)
                        .Select(s => new EventSpeakerResponse(s.Id, s.Name, s.Title, s.Company, s.PhotoUrl, s.LinkedInUrl))
                        .ToList(),
                    @event.AgendaItems
                        .OrderBy(a => a.DisplayOrder)
                        .Select(a => new EventAgendaItemResponse(a.Id, a.Title, a.Description, a.StartTime, a.EndTime))
                        .ToList());

                return Result<GetEventDetailsResponse>.Success(response, "Event retrieved successfully");
            }
        }
    }

    public record AttendeeAvatarResponse(Guid ProfessionalProfileId, string FullName, string? ProfilePictureUrl);

    public record GetEventDetailsResponse(
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
        int? AttendeeLimit,
        EventStatus Status,
        Guid CompanyId,
        string CompanyName,
        string? CompanyLogoUrl,
        int GoingCount,
        List<AttendeeAvatarResponse> AttendeeAvatars,
        bool IsRegistered,
        bool IsSaved,
        List<EventSpeakerResponse> Speakers,
        List<EventAgendaItemResponse> AgendaItems);
}