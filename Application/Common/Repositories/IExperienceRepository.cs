using Domain.Entities;

namespace Application.Common.Repositories
{
    public interface IExperienceRepository
    {
        Task AddAsync(Experience experience);
    }
}