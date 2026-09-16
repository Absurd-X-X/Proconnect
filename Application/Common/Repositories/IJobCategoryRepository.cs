using Domain.Entities;
using Domain.Enums;

namespace Application.Common.Repositories
{
    public interface IJobCategoryRepository
    {
        Task AddAsync(JobCategory category);
        Task<JobCategory?> GetByIdAsync(Guid id);
        Task<List<JobCategory>> GetAllAsync();
        Task<Dictionary<Guid, int>> GetActiveJobCountsAsync(
    EmploymentType? employmentType = null,
    ExperienceLevel? experienceLevel = null);
        Task<bool> ExistsByNameAsync(string name);
    }
}