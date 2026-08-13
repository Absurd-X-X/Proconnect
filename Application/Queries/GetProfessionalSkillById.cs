using Application.Common.Repositories;
using Application.Common.Dtos;
using MediatR;

namespace Application.Queries
{
    public class GetProfessionalSkillById
    {
        public record GetProfessionalSkillByIdQuery(Guid Id) : IRequest<Result<GetProfessionalSkillByIdResponse>>;

        public class GetProfessionalSkillByIdHandler(
            IProfessionalSkillRepository professionalSkillRepository) : IRequestHandler<GetProfessionalSkillByIdQuery, Result<GetProfessionalSkillByIdResponse>>
        {
            public async Task<Result<GetProfessionalSkillByIdResponse>> Handle(GetProfessionalSkillByIdQuery request, CancellationToken cancellationToken)
            {
                var skill = await professionalSkillRepository.GetByIdAsync(request.Id);

                if (skill is null)
                    return Result<GetProfessionalSkillByIdResponse>.Failure("Skill record not found");

                var response = new GetProfessionalSkillByIdResponse(
                    skill.Id,
                    skill.ProfessionalProfileId,
                    skill.SkillId,
                    skill.Skill.Name,
                    skill.Level,
                    skill.YearsOfExperience);

                return Result<GetProfessionalSkillByIdResponse>.Success(response, "Skill retrieved successfully");
            }
        }
    }

    public record GetProfessionalSkillByIdResponse(
        Guid Id,
        Guid ProfessionalProfileId,
        Guid SkillId,
        string SkillName,
        string Level,
        int YearsOfExperience);
}