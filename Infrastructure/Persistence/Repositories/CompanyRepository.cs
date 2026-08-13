using Application.Common.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class CompanyRepository(ProConnectDbContext proConnectDb) : ICompanyRepository
    {
        public async Task CreateAsync(Company company)
        {
            await proConnectDb.Companies.AddAsync(company);
        }

        public async Task<Company?> GetByIdAsync(Guid id)
        {
            return await proConnectDb.Companies
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }

        public async Task<Company?> GetByIdWithDetailsAsync(Guid id)
        {
            return await proConnectDb.Companies
                .Include(c => c.RecruiterProfiles.Where(r => !r.IsDeleted))
                    .ThenInclude(r => r.User)
                .Include(c => c.Jobs.Where(j => j.IsActive))
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await proConnectDb.Companies
                .AnyAsync(c => c.Name.ToLower() == name.ToLower() && !c.IsDeleted);
        }

        public async Task<Company?> GetByInvitationCodeAsync(string invitationCode)
        {
            return await proConnectDb.Companies
                .FirstOrDefaultAsync(c => c.InvitationCode == invitationCode && !c.IsDeleted);
        }

        public void UpdateAsync(Company company)
        {
            proConnectDb.Companies.Update(company);
        }

        public void Delete(Company company)
        {
            proConnectDb.Companies.Remove(company);
        }
    }
}