using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class PortfolioLinkRepository(ProConnectDbContext proConnectDb) : IPortfolioLinkRepository
    {
        public async Task CreateAsync(PortfolioLink link)
        {
            await proConnectDb.PortfolioLinks.AddAsync(link);
        }

        public async Task<PortfolioLink?> GetByIdAsync(Guid id)
        {
            return await proConnectDb.PortfolioLinks
                .FirstOrDefaultAsync(l => l.Id == id && !l.IsDeleted);
        }

        public async Task<PageResponse<PortfolioLink>> GetByProfessionalProfileIdAsync(PageRequest request, bool usePaging, Guid professionalProfileId)
        {
            var query = proConnectDb.PortfolioLinks
                .Where(l => l.ProfessionalProfileId == professionalProfileId && !l.IsDeleted)
                .OrderByDescending(l => l.DateCreated)
                .AsQueryable();

            if (usePaging)
            {
                var offset = query
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize);

                return new PageResponse<PortfolioLink>
                {
                    Items = await offset.ToListAsync(),
                    TotalCount = await query.CountAsync(),
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize
                };
            }

            var count = await query.CountAsync();

            return new PageResponse<PortfolioLink>
            {
                Items = await query.ToListAsync(),
                TotalCount = count,
                PageNumber = 1,
                PageSize = count
            };
        }

        public void UpdateAsync(PortfolioLink link)
        {
            proConnectDb.PortfolioLinks.Update(link);
        }

        public void Delete(PortfolioLink link)
        {
            proConnectDb.PortfolioLinks.Remove(link);
        }
    }
}