using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class EducationRepository(ProConnectDbContext proConnectDb) : IEducationRepository
    {
        public async Task CreateAsync(Education education)
        {
            await proConnectDb.Educations.AddAsync(education);
        }

        public async Task<Education?> GetByIdAsync(Guid id)
        {
            return await proConnectDb.Educations
                .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
        }

        public async Task<PageResponse<Education>> GetByProfessionalProfileIdAsync(PageRequest request, bool usePaging, Guid professionalProfileId)
        {
            var query = proConnectDb.Educations
                .Where(e => e.ProfessionalProfileId == professionalProfileId && !e.IsDeleted)
                .OrderByDescending(e => e.StartDate)
                .AsQueryable();

            if (usePaging)
            {
                var offset = query
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize);

                return new PageResponse<Education>
                {
                    Items = await offset.ToListAsync(),
                    TotalCount = await query.CountAsync(),
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize
                };
            }

            var count = await query.CountAsync();

            return new PageResponse<Education>
            {
                Items = await query.ToListAsync(),
                TotalCount = count,
                PageNumber = 1,
                PageSize = count
            };
        }

        public void UpdateAsync(Education education)
        {
            proConnectDb.Educations.Update(education);
        }

        public void Delete(Education education)
        {
            proConnectDb.Educations.Remove(education);
        }
    }
}