using Application.Common.Dtos;
using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.Queries.Event
{
    public class GetEventRegistrations
    {
        public record GetEventRegistrationsQuery(
            Guid UserId,
            Guid EventId,
            PageRequest PageRequest,
            bool UsePaging,
            EventRegistrationStatus? Status
        ) : IRequest<Result<PageResponse<EventRegistrationItemResponse>>>;

        public class GetEventRegistrationsHandler(
            IRecruiterProfileRepository recruiterProfileRepository,
            IEventRepository eventRepository,
            IEventRegistrationRepository eventRegistrationRepository)
            : IRequestHandler<GetEventRegistrationsQuery, Result<PageResponse<EventRegistrationItemResponse>>>
        {
            public async Task<Result<PageResponse<EventRegistrationItemResponse>>> Handle(
                GetEventRegistrationsQuery request,
                CancellationToken cancellationToken)
            {
                var recruiterProfile = await recruiterProfileRepository.GetByUserIdAsync(request.UserId);

                if (recruiterProfile is null || recruiterProfile.CompanyId is null)
                    return Result<PageResponse<EventRegistrationItemResponse>>.Failure("Recruiter profile not found");

                var @event = await eventRepository.GetByIdAsync(request.EventId);

                if (@event is null)
                    return Result<PageResponse<EventRegistrationItemResponse>>.Failure("Event not found");

                if (@event.CompanyId != recruiterProfile.CompanyId)
                    return Result<PageResponse<EventRegistrationItemResponse>>.Failure("You are not authorized to view these registrations");

                var page = await eventRegistrationRepository.GetByEventIdAsync(
                    request.PageRequest, request.UsePaging, request.EventId, request.Status);

                var items = page.Items.Select(r => new EventRegistrationItemResponse(
                    r.Id,
                    r.ProfessionalProfile.User.ProfilePictureUrl,
                    r.FullName,
                    r.Email,
                    r.Phone,
                    r.CompanyOrganization,
                    r.JobTitle,
                    r.GoalsForAttending,
                    r.ResumeUrl,
                    r.Status,
                    r.RegisteredAt))
                    .ToList();

                var response = new PageResponse<EventRegistrationItemResponse>
                {
                    Items = items,
                    TotalCount = page.TotalCount,
                    PageNumber = page.PageNumber,
                    PageSize = page.PageSize
                };

                return Result<PageResponse<EventRegistrationItemResponse>>.Success(response, "Registrations retrieved successfully");
            }
        }
    }

    public record EventRegistrationItemResponse(
        Guid Id,
        string? ProfilePictureUrl,
        string FullName,
        string Email,
        string Phone,
        string? CompanyOrganization,
        string? JobTitle,
        string? GoalsForAttending,
        string? ResumeUrl,
        EventRegistrationStatus Status,
        DateTime RegisteredAt);
}