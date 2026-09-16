using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.Commands.Job
{
    public class WithdrawApplication
    {
        public record WithdrawApplicationCommand(
            Guid ApplicationId,
            Guid ProfessionalProfileId
        ) : IRequest<Result<string>>;

        public class WithdrawApplicationHandler(
            IJobApplicationRepository jobApplicationRepository,
            IUnitOfWork unitOfWork)
            : IRequestHandler<WithdrawApplicationCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(
                WithdrawApplicationCommand request,
                CancellationToken cancellationToken)
            {
                var application = await jobApplicationRepository.GetByIdAsync(request.ApplicationId);

                if (application is null)
                    return Result<string>.Failure("Application not found");

                if (application.ProfessionalProfileId != request.ProfessionalProfileId)
                    return Result<string>.Failure("You are not authorized to withdraw this application");

                if (application.JobStatus is JobStatus.Hired or JobStatus.Rejected or JobStatus.Withdrawn)
                    return Result<string>.Failure("This application can no longer be withdrawn");

                application.JobStatus = JobStatus.Withdrawn;
                application.UpdatedAt = DateTime.UtcNow;

                jobApplicationRepository.Update(application);
                await unitOfWork.SaveAsync();

                return Result<string>.Success(application.Id.ToString(), "Application withdrawn successfully");
            }
        }
    }
}