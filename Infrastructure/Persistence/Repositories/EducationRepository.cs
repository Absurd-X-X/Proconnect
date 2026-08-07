using Application.Common.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class EducationRepository(ProConnectDbContext context) : IEducationRepository
    {
        public async Task AddAsync(Education education)
        {
            await context.Educations.AddAsync(education);
        }

        public async Task<Education?> GetByIdAsync(Guid id)
        {
            return await context.Educations.FirstOrDefaultAsync(x => x.Id == id);
        }

        public void Update(Education education)
        {
            context.Educations.Update(education);
        }
    }
}
