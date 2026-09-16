using Application.Common.Dtos;
using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.Queries.Event
{
    public class GetEventAttendees
    {
        public record GetEventAttendeesQuery(
            Guid EventId,
            PageRequest PageRequest,
            bool UsePaging
        ) : IRequest<Result<PageResponse<EventAttendeeResponse>>>;

        public class GetEventAttendeesHandler(
            IEventRepository eventRepository,
            IEventRegistrationRepository eventRegistrationRepository)
            : IRequestHandler<GetEventAttendeesQuery, Result<PageResponse<EventAttendeeResponse>>>
        {
            public async Task<Result<PageResponse<EventAttendeeResponse>>> Handle(
                GetEventAttendeesQuery request,
                CancellationToken cancellationToken)
            {
                var @event = await eventRepository.GetByIdAsync(request.EventId);

                if (@event is null)
                    return Result<PageResponse<EventAttendeeResponse>>.Failure("Event not found");

                var page = await eventRegistrationRepository.GetByEventIdAsync(
                    request.PageRequest, request.UsePaging, request.EventId, EventRegistrationStatus.Registered);

                var items = page.Items.Select(r => new EventAttendeeResponse(
                    r.ProfessionalProfile.Id,
                    r.FullName,
                    r.ProfessionalProfile.User.ProfilePictureUrl,
                    r.JobTitle,
                    r.CompanyOrganization))
                    .ToList();

                var response = new PageResponse<EventAttendeeResponse>
                {
                    Items = items,
                    TotalCount = page.TotalCount,
                    PageNumber = page.PageNumber,
                    PageSize = page.PageSize
                };

                return Result<PageResponse<EventAttendeeResponse>>.Success(response, "Attendees retrieved successfully");
            }
        }
    }

    public record EventAttendeeResponse(
        Guid ProfessionalProfileId,
        string FullName,
        string? ProfilePictureUrl,
        string? JobTitle,
        string? CompanyOrganization);
}