using Application.Common.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class JobSkillRepository(ProConnectDbContext context) : IJobSkillRepository
    {
        public async Task AddAsync(JobSkill jobSkill)
        {
            await context.JobSkills.AddAsync(jobSkill);
        }

        public async Task<JobSkill?> GetAsync(Guid jobId, Guid skillId)
        {
            return await context.JobSkills
                .FirstOrDefaultAsync(x => x.JobId == jobId && x.SkillId == skillId && !x.IsDeleted);
        }

        public async Task<List<JobSkill>> GetByJobIdAsync(Guid jobId)
        {
            return await context.JobSkills
                .AsNoTracking()
                .Include(x => x.Skill)
                .Where(x => x.JobId == jobId && !x.IsDeleted)
                .OrderBy(x => x.Skill.Name)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(Guid jobId, Guid skillId)
        {
            return await context.JobSkills
                .AnyAsync(x => x.JobId == jobId && x.SkillId == skillId && !x.IsDeleted);
        }

        public void Delete(JobSkill jobSkill)
        {
            context.JobSkills.Remove(jobSkill);
        }
    }
}