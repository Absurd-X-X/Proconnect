using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;

namespace Infrastructure.Persistence.Repositories;

public class ProjectRepository(ProConnectDbContext context) : IProjectRepository
{
    public async Task AddAsync(Project project)
    {
        await context.Projects.AddAsync(project);
    }

    public async Task<Project?> GetByIdAsync(Guid id)
    {
        return await context.Projects
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
    }

    public async Task<PageResponse<Project>> GetByProfessionalProfileIdAsync(Guid professionalProfileId, PageRequest request, bool usePaging)
    {
        var query = context.Projects
            .Where(x => x.ProfessionalProfileId == professionalProfileId && !x.IsDeleted)
            .AsNoTracking()
            .AsQueryable();

        if (!usePaging)
        {
            var allItems = await query.ToListAsync();

            return new PageResponse<Project>
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

        return new PageResponse<Project>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<PageResponse<Project>> GetAllAsync(
        PageRequest request,
        bool usePaging)
    {
        var query = context.Projects
            .AsNoTracking()
            .AsQueryable();

        if (!usePaging)
        {
            var allItems = await query.ToListAsync();

            return new PageResponse<Project>
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

        return new PageResponse<Project>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public void Update(Project project)
    {
        context.Projects.Update(project);
    }

    public void Delete(Project project)
    {
        context.Projects.Remove(project);
    }
}