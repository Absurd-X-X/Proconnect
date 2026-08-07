using Application.Common.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class CompanyRepository(
        ProConnectDbContext proConnectDb)
        : ICompanyRepository
    {
        public async Task AddAsync(Company company)
        {
            await proConnectDb.Companies.AddAsync(company);
        }


        public async Task<Company?> GetByIdAsync(Guid id)
        {
            return await proConnectDb.Companies
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}