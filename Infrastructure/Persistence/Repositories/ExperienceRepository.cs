using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class ExperienceRepository(ProConnectDbContext proConnectDb) : IExperienceRepository
    {
        public async Task CreateAsync(Experience experience)
        {
            await proConnectDb.Experiences.AddAsync(experience);
        }

        public async Task<Experience?> GetByIdAsync(Guid id)
        {
            return await proConnectDb.Experiences
                .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
        }

        public async Task<PageResponse<Experience>> GetByProfessionalProfileIdAsync(PageRequest request, bool usePaging, Guid professionalProfileId)
        {
            var query = proConnectDb.Experiences
                .Where(e => e.ProfessionalProfileId == professionalProfileId && !e.IsDeleted)
                .OrderByDescending(e => e.IsCurrentJob)
                .ThenByDescending(e => e.StartDate)
                .AsQueryable();

            if (usePaging)
            {
                var offset = query
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize);

                return new PageResponse<Experience>
                {
                    Items = await offset.ToListAsync(),
                    TotalCount = await query.CountAsync(),
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize
                };
            }

            var count = await query.CountAsync();

            return new PageResponse<Experience>
            {
                Items = await query.ToListAsync(),
                TotalCount = count,
                PageNumber = 1,
                PageSize = count
            };
        }

        public void UpdateAsync(Experience experience)
        {
            proConnectDb.Experiences.Update(experience);
        }

        public void Delete(Experience experience)
        {
            proConnectDb.Experiences.Remove(experience);
        }
    }
}