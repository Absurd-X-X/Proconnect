using Application.Common.Dtos;
using Application.Common.Repositories;
using MediatR;

namespace Application.Queries.Job
{
    public class GetJobSkills
    {
        public record GetJobSkillsQuery(Guid JobId) : IRequest<Result<List<GetJobSkillsResponse>>>;

        public class GetJobSkillsHandler(
            IJobSkillRepository jobSkillRepository)
            : IRequestHandler<GetJobSkillsQuery, Result<List<GetJobSkillsResponse>>>
        {
            public async Task<Result<List<GetJobSkillsResponse>>> Handle(
                GetJobSkillsQuery request,
                CancellationToken cancellationToken)
            {
                var jobSkills = await jobSkillRepository.GetByJobIdAsync(request.JobId);

                var response = jobSkills
                    .Select(js => new GetJobSkillsResponse(js.Id, js.SkillId, js.Skill.Name))
                    .ToList();

                return Result<List<GetJobSkillsResponse>>.Success(response, "Job skills retrieved successfully");
            }
        }
    }

    public record GetJobSkillsResponse(Guid Id, Guid SkillId, string SkillName);
}