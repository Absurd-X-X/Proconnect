using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class RecruiterProfileRepository(ProConnectDbContext proConnectDb) : IRecruiterProfileRepository
    {
        public async Task CreateAsync(RecruiterProfile recruiterProfile)
        {
            await proConnectDb.RecruiterProfiles.AddAsync(recruiterProfile);
        }

        public async Task<RecruiterProfile?> GetByIdAsync(Guid id)
        {
            return await proConnectDb.RecruiterProfiles
                .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
        }

        public async Task<RecruiterProfile?> GetByUserIdAsync(Guid userId)
        {
            return await proConnectDb.RecruiterProfiles
                .Include(r => r.Company)
                .FirstOrDefaultAsync(r => r.UserId == userId && !r.IsDeleted);
        }

        public async Task<PageResponse<RecruiterProfile>> GetByCompanyIdAsync(PageRequest request, bool usePaging, Guid companyId, RecruiterStatus? status)
        {
            var query = proConnectDb.RecruiterProfiles
                .Include(r => r.User)
                .Where(r => r.CompanyId == companyId && !r.IsDeleted);

            if (status.HasValue)
            {
                query = query.Where(r => r.Status == status.Value);
            }

            var ordered = query.OrderByDescending(r => r.DateCreated).AsQueryable();

            if (usePaging)
            {
                var offset = ordered
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize);

                return new PageResponse<RecruiterProfile>
                {
                    Items = await offset.ToListAsync(),
                    TotalCount = await ordered.CountAsync(),
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize
                };
            }

            var count = await ordered.CountAsync();

            return new PageResponse<RecruiterProfile>
            {
                Items = await ordered.ToListAsync(),
                TotalCount = count,
                PageNumber = 1,
                PageSize = count
            };
        }

        public void UpdateAsync(RecruiterProfile recruiterProfile)
        {
            proConnectDb.RecruiterProfiles.Update(recruiterProfile);
        }

        public void Delete(RecruiterProfile recruiterProfile)
        {
            proConnectDb.RecruiterProfiles.Remove(recruiterProfile);
        }
    }
}