using Application.Common.Dtos;
using Application.Common.Repositories;
using Application.Services.Interfaces;
using Domain.Enums;
using MediatR;

namespace Application.Commands.Event
{
    public class RegisterForEvent
    {
        public record RegisterForEventCommand(
            Guid UserId,
            Guid EventId,
            string FullName,
            string Email,
            string Phone,
            string? CompanyOrganization,
            string? JobTitle,
            string? GoalsForAttending,
            string? ResumeUrl,
            string? ResumePublicId
        ) : IRequest<Result<string>>;

        public class RegisterForEventHandler(
            IProfessionalProfileRepository professionalProfileRepository,
            IRecruiterProfileRepository recruiterProfileRepository,
            IEventRepository eventRepository,
            IEventRegistrationRepository eventRegistrationRepository,
            INotificationService notificationService,
            IUnitOfWork unitOfWork)
            : IRequestHandler<RegisterForEventCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(
                RegisterForEventCommand request,
                CancellationToken cancellationToken)
            {
                var professionalProfile = await professionalProfileRepository.GetByUserIdAsync(request.UserId);

                if (professionalProfile is null)
                    return Result<string>.Failure("Professional profile not found");

                var @event = await eventRepository.GetByIdAsync(request.EventId);

                if (@event is null)
                    return Result<string>.Failure("Event not found");

                if (@event.Status != EventStatus.Published)
                    return Result<string>.Failure("This event is not open for registration");

                if (@event.RegistrationType == EventRegistrationType.InviteOnly)
                    return Result<string>.Failure("This event is invite-only");

                if (@event.EndDateTime < DateTime.UtcNow)
                    return Result<string>.Failure("This event has already ended");

                var alreadyRegistered = await eventRegistrationRepository.ExistsAsync(
                    request.EventId, professionalProfile.Id);

                if (alreadyRegistered)
                    return Result<string>.Failure("You are already registered for this event");

                if (@event.AttendeeLimit.HasValue)
                {
                    var registeredCount = await eventRegistrationRepository.GetByEventIdAsync(
                        new Application.Common.Pagenation.PageRequest(),
                        usePaging: false,
                        eventId: request.EventId,
                        status: EventRegistrationStatus.Registered);

                    if (registeredCount.TotalCount >= @event.AttendeeLimit.Value)
                        return Result<string>.Failure("This event has reached its attendee limit");
                }

                var registration = new Domain.Entities.EventRegistration
                {
                    EventId = request.EventId,
                    ProfessionalProfileId = professionalProfile.Id,
                    FullName = request.FullName,
                    Email = request.Email,
                    Phone = request.Phone,
                    CompanyOrganization = request.CompanyOrganization,
                    JobTitle = request.JobTitle,
                    GoalsForAttending = request.GoalsForAttending,
                    ResumeUrl = request.ResumeUrl,
                    ResumePublicId = request.ResumePublicId,
                    CreatedBy = request.UserId.ToString()
                };

                await eventRegistrationRepository.AddAsync(registration);

                await unitOfWork.SaveAsync();

                var recruiterProfile = await recruiterProfileRepository.GetByIdAsync(@event.RecruiterProfileId);

                if (recruiterProfile is not null)
                {
                    await notificationService.SendNotificationAsync(
                        recipientUserId: recruiterProfile.UserId,
                        actorUserId: professionalProfile.UserId,
                        actorName: request.FullName,
                        actorAvatarUrl: professionalProfile.User.ProfilePictureUrl,
                        title: "New event registration",
                        message: $"{request.FullName} registered for \"{@event.Title}\"",
                        type: NotificationType.EventRegistration,
                        sourceEntityType: NotificationSourceEntityType.Event,
                        sourceEntityId: @event.Id,
                        actionUrl: $"/manage-event.html?id={@event.Id}",
                        createdBy: request.UserId.ToString());
                }

                return Result<string>.Success(registration.Id.ToString(), "Registered successfully");
            }
        }
    }
}