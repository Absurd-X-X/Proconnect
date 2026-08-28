using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class ProfessionalProfileRepository(ProConnectDbContext context) : IProfessionalProfileRepository
    {

        public async Task AddAsync(ProfessionalProfile profile)
        {
            await context.ProfessionalProfiles.AddAsync(profile);
        }

        public async Task<ProfessionalProfile?> GetByIdAsync(Guid id)
        {
            return await context.ProfessionalProfiles
                .Include(p => p.User)
                .Include(p => p.Educations)
                .Include(p => p.Experiences)
                .Include(p => p.Certificates)
                .Include(p => p.Projects)
                .Include(p => p.PortfolioLinks)
                .Include(p => p.ProfessionalSkills)
                    .ThenInclude(ps => ps.Skill)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        }

        public async Task<ProfessionalProfile?> GetByUserIdAsync(Guid userId)
        {
            return await context.ProfessionalProfiles
                .FirstOrDefaultAsync(x => x.UserId == userId && !x.IsDeleted);
        }

        public async Task<ProfessionalProfile?> GetWithDetailsAsync(Guid id)
        {
            return await context.ProfessionalProfiles
                .Include(p => p.User)
                .Include(p => p.Educations)
                .Include(p => p.Experiences)
                .Include(p => p.Certificates)
                .Include(p => p.Projects)
                .Include(p => p.PortfolioLinks)
                .Include(p => p.ProfessionalSkills)
                    .ThenInclude(ps => ps.Skill)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        }

        public async Task<PageResponse<ProfessionalProfile>> GetAllAsync(
            PageRequest request,
            bool usePaging)
        {
            var query = context.ProfessionalProfiles
                .AsNoTracking()
                .AsQueryable();

            if (!usePaging)
            {
                var allItems = await query.ToListAsync();

                return new PageResponse<ProfessionalProfile> {
                    Items = allItems,
                    TotalCount = allItems.Count,
                    PageNumber = 1,
                    PageSize = allItems.Count
                };
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return new PageResponse<ProfessionalProfile> { 
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public void Update(ProfessionalProfile profile)
        {
            context.ProfessionalProfiles.Update(profile);
        }

        public void Delete(ProfessionalProfile profile)
        {
            context.ProfessionalProfiles.Remove(profile);
        }
    }
}