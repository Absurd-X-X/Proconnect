using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class CertificateRepository(ProConnectDbContext proConnectDb) : ICertificateRepository
    {
        public async Task CreateAsync(Certificate certificate)
        {
            await proConnectDb.Certificates.AddAsync(certificate);
        }

        public async Task<Certificate?> GetByIdAsync(Guid id)
        {
            return await proConnectDb.Certificates
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }

        public async Task<PageResponse<Certificate>> GetByProfessionalProfileIdAsync(PageRequest request, bool usePaging, Guid professionalProfileId)
        {
            var query = proConnectDb.Certificates
                .Where(c => c.ProfessionalProfileId == professionalProfileId && !c.IsDeleted)
                .OrderByDescending(c => c.IssueDate)
                .AsQueryable();

            if (usePaging)
            {
                var offset = query
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize);

                return new PageResponse<Certificate>
                {
                    Items = await offset.ToListAsync(),
                    TotalCount = await query.CountAsync(),
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize
                };
            }

            var count = await query.CountAsync();

            return new PageResponse<Certificate>
            {
                Items = await query.ToListAsync(),
                TotalCount = count,
                PageNumber = 1,
                PageSize = count
            };
        }

        public void UpdateAsync(Certificate certificate)
        {
            proConnectDb.Certificates.Update(certificate);
        }

        public void Delete(Certificate certificate)
        {
            proConnectDb.Certificates.Remove(certificate);
        }
    }
}