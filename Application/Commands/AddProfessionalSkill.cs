using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Commands
{
    public class AddProfessionalSkill
    {
        public record AddProfessionalSkillCommand(
            Guid ProfessionalProfileId,
            Guid SkillId,
            string Level,
            int YearsOfExperience
        ) : IRequest<Result<string>>;

        public class AddProfessionalSkillHandler(
            IProfessionalSkillRepository professionalSkillRepository,
            IUnitOfWork unitOfWork)
            : IRequestHandler<
                AddProfessionalSkill.AddProfessionalSkillCommand,
                Result<string>>
        {
            public async Task<Result<string>> Handle(
                AddProfessionalSkill.AddProfessionalSkillCommand request,
                CancellationToken cancellationToken)
            {
                var exists = await professionalSkillRepository.ExistsAsync(
                    request.ProfessionalProfileId,
                    request.SkillId);


                if (exists)
                {
                    return Result<string>.Failure(
                        "Skill already added");
                }


                var professionalSkill = new ProfessionalSkill
                {
                    ProfessionalProfileId = request.ProfessionalProfileId,

                    SkillId = request.SkillId,

                    Level = request.Level,

                    YearsOfExperience = request.YearsOfExperience,

                    DateCreated = DateTime.UtcNow
                };


                await professionalSkillRepository.AddAsync(professionalSkill);

                await unitOfWork.SaveAsync();


                return Result<string>.Success(
                    "Skill added successfully",
                    "created");
            }
        }
    }
}