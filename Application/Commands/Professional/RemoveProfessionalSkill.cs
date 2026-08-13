using Application.Common.Repositories;
using Application.Common.Dtos;
using MediatR;

namespace Application.Commands.Professional
{
    public class RemoveProfessionalSkill
    {
        public record RemoveProfessionalSkillCommand(Guid Id) : IRequest<Result<string>>;

        public class RemoveProfessionalSkillHandler(
            IProfessionalSkillRepository professionalSkillRepository,
            IUnitOfWork unitOfWork) : IRequestHandler<RemoveProfessionalSkillCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(RemoveProfessionalSkillCommand request, CancellationToken cancellationToken)
            {
                var skill = await professionalSkillRepository.GetByIdAsync(request.Id);

                if (skill is null)
                    return Result<string>.Failure("Skill record not found");

                professionalSkillRepository.Delete(skill);

                await unitOfWork.SaveAsync();

                return Result<string>.Success("Skill removed successfully", "removed");
            }
        }
    }
}