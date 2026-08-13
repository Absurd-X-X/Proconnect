using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class ProfessionalSkillRepository(ProConnectDbContext proConnectDb) : IProfessionalSkillRepository
    {
        public async Task CreateAsync(ProfessionalSkill skill)
        {
            await proConnectDb.ProfessionalSkills.AddAsync(skill);
        }

        public async Task<ProfessionalSkill?> GetByIdAsync(Guid id)
        {
            return await proConnectDb.ProfessionalSkills
                .Include(ps => ps.Skill)
                .FirstOrDefaultAsync(ps => ps.Id == id && !ps.IsDeleted);
        }

        public async Task<PageResponse<ProfessionalSkill>> GetByProfessionalProfileIdAsync(PageRequest request, bool usePaging, Guid professionalProfileId)
        {
            var query = proConnectDb.ProfessionalSkills
                .Include(ps => ps.Skill)
                .Where(ps => ps.ProfessionalProfileId == professionalProfileId && !ps.IsDeleted)
                .AsQueryable();

            if (usePaging)
            {
                var offset = query
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize);

                return new PageResponse<ProfessionalSkill>
                {
                    Items = await offset.ToListAsync(),
                    TotalCount = await query.CountAsync(),
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize
                };
            }

            var count = await query.CountAsync();

            return new PageResponse<ProfessionalSkill>
            {
                Items = await query.ToListAsync(),
                TotalCount = count,
                PageNumber = 1,
                PageSize = count
            };
        }

        public async Task<bool> ExistsAsync(Guid professionalProfileId, Guid skillId)
        {
            return await proConnectDb.ProfessionalSkills
                .AnyAsync(ps => ps.ProfessionalProfileId == professionalProfileId && ps.SkillId == skillId && !ps.IsDeleted);
        }

        public void Delete(ProfessionalSkill skill)
        {
            proConnectDb.ProfessionalSkills.Remove(skill);
        }
    }
}