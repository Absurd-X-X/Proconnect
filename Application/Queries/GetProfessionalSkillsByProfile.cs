using Application.Common.Pagenation;
using Application.Common.Repositories;
using Application.Common.Dtos;
using MediatR;

namespace Application.Queries
{
    public class GetProfessionalSkillsByProfile
    {
        public record GetProfessionalSkillsByProfileQuery(
            Guid ProfessionalProfileId,
            PageRequest PageRequest,
            bool UsePaging
            ) : IRequest<Result<PageResponse<GetProfessionalSkillsByProfileRespone>>>;

        public class GetProfessionalSkillsByProfileHandler(
            IProfessionalSkillRepository professionalSkillRepository) : IRequestHandler<GetProfessionalSkillsByProfileQuery, Result<PageResponse<GetProfessionalSkillsByProfileRespone>>>
        {
            public async Task<Result<PageResponse<GetProfessionalSkillsByProfileRespone>>> Handle(GetProfessionalSkillsByProfileQuery request, CancellationToken cancellationToken)
            {
                var skills = await professionalSkillRepository.GetByProfessionalProfileIdAsync(
                    request.PageRequest,
                    request.UsePaging,
                    request.ProfessionalProfileId);

                var items = skills.Items.Select(s => new GetProfessionalSkillsByProfileRespone(
                    s.Id,
                    s.SkillId,
                    s.Skill.Name,
                    s.Level,
                    s.YearsOfExperience)).ToList();

                var response = new PageResponse<GetProfessionalSkillsByProfileRespone>
                {
                    Items = items,
                    TotalCount = skills.TotalCount,
                    PageNumber = skills.PageNumber,
                    PageSize = skills.PageSize
                };

                return Result<PageResponse<GetProfessionalSkillsByProfileRespone>>.Success(response, "Skills retrieved successfully");
            }
        }
    }

    public record GetProfessionalSkillsByProfileRespone(
        Guid Id,
        Guid SkillId,
        string SkillName,
        string Level,
        int YearsOfExperience);
}