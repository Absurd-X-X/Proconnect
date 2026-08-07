using Application.Common.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;

namespace Infrastructure.Persistence.Repositories
{
    public class ExperienceRepository(ProConnectDbContext context) : IExperienceRepository
    {
        public async Task AddAsync(Experience experience)
        {
            await context.Experiences.AddAsync(experience);
        }
    }
}
