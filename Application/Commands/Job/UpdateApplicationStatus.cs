using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Commands.Job
{
    public class UpdateApplicationStatus
    {
        public record UpdateApplicationStatusCommand(
            Guid ApplicationId,
            Guid RecruiterProfileId,
            JobStatus NewStatus
        ) : IRequest<Result<string>>;

        public class UpdateApplicationStatusHandler(
            IJobApplicationRepository jobApplicationRepository,
            IRecruiterProfileRepository recruiterProfileRepository,
            IUnitOfWork unitOfWork)
            : IRequestHandler<UpdateApplicationStatusCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(
                UpdateApplicationStatusCommand request,
                CancellationToken cancellationToken)
            {
                var application = await jobApplicationRepository.GetByIdAsync(request.ApplicationId);

                if (application is null)
                    return Result<string>.Failure("Application not found");

                var callingRecruiter = await recruiterProfileRepository.GetByIdAsync(request.RecruiterProfileId);

                if (callingRecruiter is null || callingRecruiter.Status != RecruiterStatus.Active)
                    return Result<string>.Failure("You are not authorized to update this application");

                if (callingRecruiter.CompanyId != application.Job.CompanyId)
                    return Result<string>.Failure("You are not authorized to update this application");

                if (application.JobStatus == JobStatus.Withdrawn)
                    return Result<string>.Failure("This application was withdrawn by the candidate and can no longer be updated");

                if (request.NewStatus == JobStatus.Withdrawn)
                    return Result<string>.Failure("Recruiters cannot withdraw an application on a candidate's behalf");

                application.JobStatus = request.NewStatus;
                application.UpdatedAt = DateTime.UtcNow;
                application.LastActionedByRecruiterProfileId = request.RecruiterProfileId;

                await unitOfWork.SaveAsync();

                return Result<string>.Success(application.Id.ToString(), "Application status updated successfully");
            }
        }
    }
}