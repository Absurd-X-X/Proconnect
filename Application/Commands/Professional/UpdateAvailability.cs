using Application.Common.Repositories;
using Application.Common.Dtos;
using Domain.Enums;
using MediatR;

namespace Application.Commands.Professional
{
    public class UpdateAvailability
    {
        public record UpdateAvailabilityCommand(
            Guid UserId,
            UserStatus UserStatus,
            List<EmploymentType> PreferredJobTypes,
            List<string> PreferredLocations,
            DateTime? EarliestStartDate,
            bool WillingToRelocate,
            WorkAuthorizationStatus WorkAuthorization,
            AvailabilityVisibility AvailabilityVisibility
            ) : IRequest<Result<string>>;

        public class UpdateAvailabilityHandler(
            IProfessionalProfileRepository professionalProfileRepository,
            IUnitOfWork unitOfWork) : IRequestHandler<UpdateAvailabilityCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(UpdateAvailabilityCommand request, CancellationToken cancellationToken)
            {
                var profile = await professionalProfileRepository.GetByUserIdAsync(request.UserId);

                if (profile is null)
                    return Result<string>.Failure("Professional profile not found");

                profile.UserStatus = request.UserStatus;

                profile.PreferredJobTypes = request.PreferredJobTypes;

                profile.PreferredLocations = request.PreferredLocations;

                profile.EarliestStartDate = request.EarliestStartDate;

                profile.WillingToRelocate = request.WillingToRelocate;

                profile.WorkAuthorization = request.WorkAuthorization;

                profile.AvailabilityVisibility = request.AvailabilityVisibility;

                profile.DateModified = DateTime.UtcNow;

                professionalProfileRepository.Update(profile);

                await unitOfWork.SaveAsync();

                return Result<string>.Success("Availability updated successfully", "updated");
            }
        }
    }
}