using Application.Common.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class ProfessionalSkillRepository(
        ProConnectDbContext proConnectDb)
        : IProfessionalSkillRepository
    {
        public async Task AddAsync(ProfessionalSkill professionalSkill)
        {
            await proConnectDb.ProfessionalSkills.AddAsync(professionalSkill);
        }


        public async Task<bool> ExistsAsync(
            Guid professionalProfileId,
            Guid skillId)
        {
            return await proConnectDb.ProfessionalSkills
                .AnyAsync(x =>
                    x.ProfessionalProfileId == professionalProfileId &&
                    x.SkillId == skillId);
        }
    }
}