using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class CompanyReviewRepository(ProConnectDbContext context) : ICompanyReviewRepository
    {
        public async Task AddAsync(CompanyReview review)
        {
            await context.CompanyReviews.AddAsync(review);
        }

        public async Task<CompanyReview?> GetByIdAsync(Guid id)
        {
            return await context.CompanyReviews
                .Include(r => r.Reviewer)
                .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
        }

        public async Task<bool> ExistsAsync(Guid companyId, Guid reviewerId)
        {
            return await context.CompanyReviews
                .AnyAsync(r => r.CompanyId == companyId && r.ReviewerId == reviewerId && !r.IsDeleted);
        }

        public async Task<PageResponse<CompanyReview>> GetByCompanyIdAsync(
            Guid companyId, PageRequest request, bool usePaging)
        {
            var query = context.CompanyReviews
                .AsNoTracking()
                .Include(r => r.Reviewer)
                .Where(r => r.CompanyId == companyId && !r.IsDeleted)
                .OrderByDescending(r => r.DateCreated)
                .AsQueryable();

            if (!usePaging)
            {
                var allItems = await query.ToListAsync();

                return new PageResponse<CompanyReview>
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

            return new PageResponse<CompanyReview>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<List<CompanyReview>> GetAllByCompanyIdAsync(Guid companyId)
        {
            return await context.CompanyReviews
                .AsNoTracking()
                .Where(r => r.CompanyId == companyId && !r.IsDeleted)
                .ToListAsync();
        }

        public void IncrementHelpful(CompanyReview review)
        {
            review.HelpfulCount += 1;
            context.CompanyReviews.Update(review);
        }
    }
}