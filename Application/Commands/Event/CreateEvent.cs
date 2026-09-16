using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.Commands.Event
{
    public class CreateEvent
    {
        public record CreateEventCommand(
            Guid UserId,
            string Title,
            string Description,
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
            bool SaveAsDraft
        ) : IRequest<Result<string>>;

        public class CreateEventHandler(
            IRecruiterProfileRepository recruiterProfileRepository,
            IEventRepository eventRepository,
            IUnitOfWork unitOfWork)
            : IRequestHandler<CreateEventCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(
                CreateEventCommand request,
                CancellationToken cancellationToken)
            {
                var recruiterProfile = await recruiterProfileRepository.GetByUserIdAsync(request.UserId);

                if (recruiterProfile is null)
                    return Result<string>.Failure("Recruiter profile not found");

                if (recruiterProfile.CompanyId is null)
                    return Result<string>.Failure("You must be linked to a company to create events");

                if (request.EndDateTime <= request.StartDateTime)
                    return Result<string>.Failure("End date/time must be after start date/time");

                if (request.LocationType is Domain.Enums.EventLocationType.InPerson or Domain.Enums.EventLocationType.Hybrid
                    && string.IsNullOrWhiteSpace(request.Location))
                    return Result<string>.Failure("Location is required for in-person or hybrid events");

                if (request.LocationType is Domain.Enums.EventLocationType.Online or Domain.Enums.EventLocationType.Hybrid
                    && string.IsNullOrWhiteSpace(request.OnlineMeetingLink))
                    return Result<string>.Failure("Online meeting link is required for online or hybrid events");

                var @event = new Domain.Entities.Event
                {
                    CompanyId = recruiterProfile.CompanyId.Value,
                    RecruiterProfileId = recruiterProfile.Id,
                    Title = request.Title,
                    Description = request.Description,
                    EventType = request.EventType,
                    Category = request.Category,
                    StartDateTime = request.StartDateTime,
                    EndDateTime = request.EndDateTime,
                    LocationType = request.LocationType,
                    Location = request.Location,
                    OnlineMeetingLink = request.OnlineMeetingLink,
                    Visibility = request.Visibility,
                    RegistrationType = request.RegistrationType,
                    AttendeeLimit = request.AttendeeLimit,
                    Status = request.SaveAsDraft ? EventStatus.Draft : EventStatus.Published,
                    PublishedAt = request.SaveAsDraft ? null : DateTime.UtcNow,
                    CreatedBy = request.UserId.ToString()
                };

                await eventRepository.AddAsync(@event);

                await unitOfWork.SaveAsync();

                return Result<string>.Success(
                    @event.Id.ToString(),
                    request.SaveAsDraft ? "Event saved as draft" : "Event published successfully");
            }
        }
    }
}