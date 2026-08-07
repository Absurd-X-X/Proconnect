using Application.Common.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class ProfessionalProfileRepository(ProConnectDbContext context) : IProfessionalProfileRepository
    {
        public async Task AddProfessionalProfile(ProfessionalProfile profile)
        {
            await context.professionalProfiles.AddAsync(profile);
        }

        public async Task<ProfessionalProfile?> GetByUserIdAsync(Guid userId)
        {
            return await context.professionalProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public void Update(ProfessionalProfile profile)
        {
            context.professionalProfiles.Update(profile);
        }
    }
}
