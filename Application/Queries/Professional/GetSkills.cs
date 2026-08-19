using Application.Common.Dtos;
using Application.Common.Repositories;
using MediatR;

namespace Application.Queries.Skills
{
    public class GetSkills
    {
        public record SkillDto(Guid Id, string Name);

        public record GetSkillsQuery() : IRequest<Result<List<SkillDto>>>;

        public class GetSkillsHandler(ISkillRepository skillRepository)
            : IRequestHandler<GetSkillsQuery, Result<List<SkillDto>>>
        {
            public async Task<Result<List<SkillDto>>> Handle(
                GetSkillsQuery request,
                CancellationToken cancellationToken)
            {
                var skills = await skillRepository.GetAllAsync();

                var dtos = skills
                    .OrderBy(s => s.Name)
                    .Select(s => new SkillDto(s.Id, s.Name))
                    .ToList();

                return Result<List<SkillDto>>.Success(dtos, "retrieved");
            }
        }
    }
}