using Domain.Entities;

namespace Application.Common.Repositories
{
    public interface IJobSkillRepository
    {
        Task AddAsync(JobSkill jobSkill);
        Task<JobSkill?> GetAsync(Guid jobId, Guid skillId);
        Task<List<JobSkill>> GetByJobIdAsync(Guid jobId);
        Task<bool> ExistsAsync(Guid jobId, Guid skillId);
        void Delete(JobSkill jobSkill);
    }
}