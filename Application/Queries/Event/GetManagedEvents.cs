using Application.Common.Dtos;
using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.Queries.Event
{
    public class GetManagedEvents
    {
        public record GetManagedEventsQuery(
    Guid UserId,
    PageRequest PageRequest,
    bool UsePaging,
    EventStatus? Status,
    EventType? EventType,
    string? SearchTerm
) : IRequest<Result<GetManagedEventsResponse>>;

        public class GetManagedEventsHandler(
            IRecruiterProfileRepository recruiterProfileRepository,
            IEventRepository eventRepository)
            : IRequestHandler<GetManagedEventsQuery, Result<GetManagedEventsResponse>>
        {
            public async Task<Result<GetManagedEventsResponse>> Handle(
                GetManagedEventsQuery request,
                CancellationToken cancellationToken)
            {
                var recruiterProfile = await recruiterProfileRepository.GetByUserIdAsync(request.UserId);

                if (recruiterProfile is null)
                    return Result<GetManagedEventsResponse>.Failure("Recruiter profile not found");

                var page = await eventRepository.GetByRecruiterProfileIdAsync(
                    request.PageRequest, request.UsePaging, recruiterProfile.Id, request.Status, request.EventType);

                // ...

                var allForCounts = await eventRepository.GetByRecruiterProfileIdAsync(
                    new PageRequest(), usePaging: false, recruiterProfile.Id, status: null, eventType: request.EventType);

                var items = page.Items.Select(e => new ManagedEventItemResponse(
                    e.Id,
                    e.Title,
                    e.EventType,
                    e.Category,
                    e.CoverImageUrl,
                    e.StartDateTime,
                    e.EndDateTime,
                    e.LocationType,
                    e.Location,
                    e.Status,
                    e.Registrations.Count(r => r.Status != EventRegistrationStatus.CancelledByUser),
                    e.Registrations.Count(r => r.Status == EventRegistrationStatus.Attended)))
                    .ToList();


                var counts = new EventStatusCountsResponse(
                    allForCounts.TotalCount,
                    allForCounts.Items.Count(e => e.Status == EventStatus.Published),
                    allForCounts.Items.Count(e => e.Status == EventStatus.Draft),
                    allForCounts.Items.Count(e => e.Status == EventStatus.Cancelled),
                    allForCounts.Items.Count(e => e.Status == EventStatus.Completed));

                var response = new GetManagedEventsResponse(
                    items, counts, page.TotalCount, page.PageNumber, page.PageSize);

                return Result<GetManagedEventsResponse>.Success(response, "Managed events retrieved successfully");
            }
        }
    }

    public record ManagedEventItemResponse(
        Guid Id,
        string Title,
        EventType EventType,
        string? Category,
        string? CoverImageUrl,
        DateTime StartDateTime,
        DateTime EndDateTime,
        EventLocationType LocationType,
        string? Location,
        EventStatus Status,
        int RegistrationCount,
        int AttendedCount);

    public record EventStatusCountsResponse(
        int All,
        int Published,
        int Drafts,
        int Cancelled,
        int Completed);

    public record GetManagedEventsResponse(
        List<ManagedEventItemResponse> Items,
        EventStatusCountsResponse StatusCounts,
        int TotalCount,
        int PageNumber,
        int PageSize);
}