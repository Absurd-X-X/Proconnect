using Application.Common.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class SkillRepository(ProConnectDbContext context) : ISkillRepository
    {
        public async Task<List<Skill>> GetAllAsync()
        {
            return await context.Skills
                .Where(s => !s.IsDeleted)
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<Skill?> GetByIdAsync(Guid id)
        {
            return await context.Skills
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
        }

        public async Task AddAsync(Skill skill)
        {
            await context.Skills.AddAsync(skill);
        }
    }
}