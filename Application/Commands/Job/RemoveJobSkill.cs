using Application.Common.Dtos;
using Application.Common.Repositories;
using MediatR;

namespace Application.Commands.Job
{
    public class RemoveJobSkill
    {
        public record RemoveJobSkillCommand(
            Guid JobId,
            Guid RecruiterProfileId,
            Guid SkillId
        ) : IRequest<Result<string>>;

        public class RemoveJobSkillHandler(
            IJobRepository jobRepository,
            IJobSkillRepository jobSkillRepository,
            IUnitOfWork unitOfWork)
            : IRequestHandler<RemoveJobSkillCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(
                RemoveJobSkillCommand request,
                CancellationToken cancellationToken)
            {
                var job = await jobRepository.GetByIdAsync(request.JobId);

                if (job is null)
                    return Result<string>.Failure("Job not found");

                if (job.RecruiterProfileId != request.RecruiterProfileId)
                    return Result<string>.Failure("You are not authorized to edit this job");

                var jobSkill = await jobSkillRepository.GetAsync(request.JobId, request.SkillId);

                if (jobSkill is null)
                    return Result<string>.Failure("Skill not found on this job");

                jobSkillRepository.Delete(jobSkill);

                await unitOfWork.SaveAsync();

                return Result<string>.Success("Skill removed successfully", "removed");
            }
        }
    }
}