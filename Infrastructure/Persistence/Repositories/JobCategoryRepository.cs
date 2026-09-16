using Application.Common.Repositories;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class JobCategoryRepository(ProConnectDbContext context) : IJobCategoryRepository
    {
        public async Task AddAsync(JobCategory category)
        {
            await context.JobCategories.AddAsync(category);
        }

        public async Task<JobCategory?> GetByIdAsync(Guid id)
        {
            return await context.JobCategories
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }

        public async Task<List<JobCategory>> GetAllAsync()
        {
            return await context.JobCategories
                .AsNoTracking()
                .Where(c => !c.IsDeleted)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Dictionary<Guid, int>> GetActiveJobCountsAsync(
    EmploymentType? employmentType = null,
    ExperienceLevel? experienceLevel = null)
        {
            var query = context.Jobs
                .AsNoTracking()
                .Where(j => j.Status == JobPostingStatus.Active)
                .AsQueryable();

            if (employmentType.HasValue)
                query = query.Where(j => j.EmploymentType == employmentType.Value);

            if (experienceLevel.HasValue)
                query = query.Where(j => j.ExperienceLevel == experienceLevel.Value);

            return await query
                .GroupBy(j => j.JobCategoryId)
                .Select(g => new { CategoryId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.CategoryId, x => x.Count);
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await context.JobCategories
                .AnyAsync(c => c.Name.ToLower() == name.ToLower() && !c.IsDeleted);
        }
    }
}