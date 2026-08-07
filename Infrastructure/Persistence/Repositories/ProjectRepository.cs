using Application.Common.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;

namespace Infrastructure.Persistence.Repositories
{
    public class ProjectRepository(
        ProConnectDbContext proConnectDb)
        : IProjectRepository
    {
        public async Task AddAsync(Project project)
        {
            await proConnectDb.Projects.AddAsync(project);
        }
    }
}