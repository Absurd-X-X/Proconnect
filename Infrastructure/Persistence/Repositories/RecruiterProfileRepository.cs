using Application.Common.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class RecruiterProfileRepository(
        ProConnectDbContext proConnectDb)
        : IRecruiterProfileRepository
    {
        public async Task AddAsync(
            RecruiterProfile recruiterProfile)
        {
            await proConnectDb.RecruiterProfiles
                .AddAsync(recruiterProfile);
        }

        public async Task<RecruiterProfile?> GetByUserIdAsync(Guid userId)
        {
            return await proConnectDb.RecruiterProfiles
                .FirstOrDefaultAsync(r => r.UserId == userId);
        }
    }
}