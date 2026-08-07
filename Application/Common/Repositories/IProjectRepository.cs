using Domain.Entities;

namespace Application.Common.Repositories
{
    public interface IProjectRepository
    {
        Task AddAsync(Project project);
    }
}