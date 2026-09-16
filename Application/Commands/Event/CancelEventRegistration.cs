using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.Commands.Event
{
    public class CancelEventRegistration
    {
        public record CancelEventRegistrationCommand(Guid UserId, Guid EventId) : IRequest<Result<string>>;

        public class CancelEventRegistrationHandler(
            IProfessionalProfileRepository professionalProfileRepository,
            IEventRegistrationRepository eventRegistrationRepository,
            IUnitOfWork unitOfWork)
            : IRequestHandler<CancelEventRegistrationCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(
                CancelEventRegistrationCommand request,
                CancellationToken cancellationToken)
            {
                var professionalProfile = await professionalProfileRepository.GetByUserIdAsync(request.UserId);

                if (professionalProfile is null)
                    return Result<string>.Failure("Professional profile not found");

                var registration = await eventRegistrationRepository.GetByEventAndProfileAsync(
                    request.EventId, professionalProfile.Id);

                if (registration is null)
                    return Result<string>.Failure("You are not registered for this event");

                registration.Status = EventRegistrationStatus.CancelledByUser;
                registration.CancelledAt = DateTime.UtcNow;

                eventRegistrationRepository.Update(registration);

                await unitOfWork.SaveAsync();

                return Result<string>.Success("cancelled", "Registration cancelled");
            }
        }
    }
}