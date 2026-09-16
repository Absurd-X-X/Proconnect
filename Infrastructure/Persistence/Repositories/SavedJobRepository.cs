using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class SavedJobRepository(ProConnectDbContext context) : ISavedJobRepository
    {
        public async Task AddAsync(SavedJob savedJob)
        {
            await context.SavedJobs.AddAsync(savedJob);
        }

        public async Task<SavedJob?> GetByProfessionalAndJobAsync(Guid professionalProfileId, Guid jobId)
        {
            return await context.SavedJobs
                .FirstOrDefaultAsync(s =>
                    s.ProfessionalProfileId == professionalProfileId &&
                    s.JobId == jobId &&
                    !s.IsDeleted);
        }

        public async Task<PageResponse<SavedJob>> GetByProfessionalProfileIdAsync(
            Guid professionalProfileId,
            PageRequest request,
            bool usePaging)
        {
            var query = context.SavedJobs
                .AsNoTracking()
                .Include(s => s.Job)
                    .ThenInclude(j => j.Company)
                .Where(s => s.ProfessionalProfileId == professionalProfileId && !s.IsDeleted)
                .OrderByDescending(s => s.SavedAt)
                .AsQueryable();

            if (!usePaging)
            {
                var allItems = await query.ToListAsync();

                return new PageResponse<SavedJob>
                {
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

            return new PageResponse<SavedJob>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public void Delete(SavedJob savedJob)
        {
            context.SavedJobs.Remove(savedJob);
        }
    }
}