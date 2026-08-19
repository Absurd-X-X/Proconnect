using Domain.Entities;

namespace Application.Common.Repositories
{
    public interface ISkillRepository
    {
        Task<List<Skill>> GetAllAsync();
        Task<Skill?> GetByIdAsync(Guid id);
        Task AddAsync(Skill skill);
    }
}