using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.Commands.Job
{
    public class CloseJob
    {
        public record CloseJobCommand(
            Guid JobId,
            Guid RecruiterProfileId
        ) : IRequest<Result<string>>;

        public class CloseJobHandler(
            IJobRepository jobRepository,
            IUnitOfWork unitOfWork)
            : IRequestHandler<CloseJobCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(
                CloseJobCommand request,
                CancellationToken cancellationToken)
            {
                var job = await jobRepository.GetByIdAsync(request.JobId);

                if (job is null)
                    return Result<string>.Failure("Job not found");

                if (job.RecruiterProfileId != request.RecruiterProfileId)
                    return Result<string>.Failure("You are not authorized to close this job");

                if (job.Status == JobPostingStatus.Closed)
                    return Result<string>.Failure("This job is already closed");

                job.Status = JobPostingStatus.Closed;

                job.IsActive = false;

                job.UpdatedAt = DateTime.UtcNow;

                jobRepository.Update(job);

                await unitOfWork.SaveAsync();

                return Result<string>.Success(job.Id.ToString(), "Job closed successfully");
            }
        }
    }
}